namespace StoneColdTetris.Core.Game;

public class Renderer
{
    private enum CellStyle { Empty, Filled, Ghost, Flash }
    
    public readonly int BoardRow = 1;
    public readonly int BoardColumn = 1;
    public Dictionary<PieceType, ConsoleColor> ColourMap = new Dictionary<PieceType, ConsoleColor>
    {
        { PieceType.I, ConsoleColor.Cyan },
        { PieceType.J, ConsoleColor.Blue },
        { PieceType.L, ConsoleColor.DarkYellow },
        { PieceType.O, ConsoleColor.Yellow },
        { PieceType.S, ConsoleColor.Green },
        { PieceType.T, ConsoleColor.Magenta },
        { PieceType.Z, ConsoleColor.Red} 
        
    };
    
    private const ConsoleColor DefaultColor = ConsoleColor.DarkGray;
    private const int CellWidth = 4;
    const int CellHeight = 2;
    private const int PreviewSize = 4; // cells across and down; fits the largest piece (I)
    public void Init()
    {
        Console.CursorVisible = false;
        Console.Title = "Tetris";
        Console.Clear();
        Console.ForegroundColor = DefaultColor;
    }
    
    
    void DrawBoardCell(int x, int y, CellStyle style, PieceType? type = null) =>
        DrawCell((x + BoardColumn) * CellWidth, (y + BoardRow) * CellHeight, style, type);

    void DrawCell(int left, int top, CellStyle style, PieceType? type = null)   // screen chars, no offsets
    {
        // each style picks a fill and the first 3 chars of the top row; the layout below is shared
        ConsoleColor fill;
        string topRow;
        switch (style)
        {
            case CellStyle.Filled:
                fill = ColourMap[type!.Value];
                topRow = "▀  ";
                break;
            case CellStyle.Ghost:
                fill = ConsoleColor.DarkGray;
                topRow = "   ";
                break;
            case CellStyle.Flash:
                fill = ConsoleColor.White;
                topRow = "   ";
                break;
            case CellStyle.Empty:
            default:
                fill = ConsoleColor.Black;
                topRow = "   ";
                break;
        }

        Console.SetCursorPosition(left, top);
        Console.BackgroundColor = fill;
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write(topRow);
        Console.BackgroundColor = ConsoleColor.Black;
        Console.Write(" ");

        // fill comes from the background so it spans the full cell; the glyph only draws the black outline
        Console.SetCursorPosition(left, top + 1);
        Console.ForegroundColor = ConsoleColor.Black;
        Console.BackgroundColor = fill;
        Console.Write("▄▄▄");
        Console.BackgroundColor = ConsoleColor.Black;
        Console.Write(" ");

        Console.ForegroundColor = DefaultColor;
    }

    private void DrawBoard(Board board)
    {
        for (int y = 0; y < board.Height; y++)
        {
            for (int x = 0; x < board.Width; x++)
            {
                bool filled = board.IsCellFilled(x, y);
                DrawBoardCell(x, y, filled ? CellStyle.Filled : CellStyle.Empty,
                    filled ? board.GetCellPieceType(x, y) : null);
            }
        }

        int leftWall = BoardColumn * CellWidth - 2;
        int rightWall = (BoardColumn + board.Width) * CellWidth;
        int topBar = BoardRow * CellHeight - 1;
        int bottomBar = (BoardRow + board.Height) * CellHeight;
        string bar = new string('═', rightWall - leftWall - 1);

        for (int row = topBar + 1; row < bottomBar; row++)
        {
            Console.SetCursorPosition(leftWall, row);
            Console.Write("║");
            Console.SetCursorPosition(rightWall, row);
            Console.Write("║");
        }

        Console.SetCursorPosition(leftWall, topBar);
        Console.Write("╔" + bar + "╗");
        Console.SetCursorPosition(leftWall, bottomBar);
        Console.Write("╚" + bar + "╝");
    }

    public void Draw(Board board, ActivePiece activePiece, int points, GamePiece nextPiece, 
        ActivePiece dropPreviewPiece, GamePiece? storedPiece, List<int> clearingRows, bool flashOn)    
    {
        DrawBoard(board);

        // side panel, positioned in screen chars from the board's right wall
        int panelLeft = (BoardColumn + board.Width) * CellWidth + 3;
        DrawPreview("Stored Piece:", storedPiece, panelLeft, 1);
        DrawText("Points:", panelLeft, 12);
        DrawText(points.ToString(), panelLeft, 13);
        DrawPreview("Next Piece:", nextPiece, panelLeft, 16);

        if (clearingRows.Count > 0 && flashOn)
        {
            FlashRows(clearingRows, board);
            return;
        }
        
        //DRAW DROP PREVIEW
        DrawPiece(dropPreviewPiece.Cells(), dropPreviewPiece.PieceType, CellStyle.Ghost);
        
        //DRAW ACTIVE PIECE
        DrawPiece(activePiece.Cells(), activePiece.PieceType, CellStyle.Filled);
    }

    private void FlashRows(List<int> rowsToClear, Board board)
    {
        for (int r = 0; r < rowsToClear.Count; r++)
        {
            for (int x = 0; x < board.Width; x++)
            {
                DrawBoardCell(x,rowsToClear[r],CellStyle.Flash,board.GetCellPieceType(x,rowsToClear[r]));
            }            
        }
    }

    // padded to the panel width so shorter text overwrites longer text from earlier frames
    private void DrawText(string text, int left, int top)
    {
        Console.SetCursorPosition(left, top);
        Console.Write(text.PadRight(PreviewSize * CellWidth));
    }

    // draws the full 4x4 box every frame, so empty cells overwrite whatever the previous piece left
    private void DrawPreview(string label, GamePiece? piece, int left, int top)
    {
        DrawText(label, left, top);
        for (int row = 0; row < PreviewSize; row++)
        {
            for (int col = 0; col < PreviewSize; col++)
            {
                bool filled = piece != null
                    && row < piece.PieceData.GetLength(0)
                    && col < piece.PieceData.GetLength(1)
                    && piece.PieceData[row, col] == 1;
                DrawCell(left + col * CellWidth, top + 1 + row * CellHeight,
                    filled ? CellStyle.Filled : CellStyle.Empty, piece?.PieceType);
            }
        }
    }

    private void DrawPiece(List<Cell> pieceCells, PieceType type, CellStyle style)
    {
        foreach (Cell coord in pieceCells)
        {
            DrawBoardCell(coord.X, coord.Y, style, type);
        }
    }
}