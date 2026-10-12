namespace StoneColdTetris.Core.Game;

public class Renderer
{
    public readonly int BoardRow = 1;
    public readonly int BoardColumn = 1;
    public Dictionary<PieceType, (ConsoleColor, ConsoleColor)> ColourMap = new Dictionary<PieceType, (ConsoleColor, ConsoleColor)>
    {
        { PieceType.I, (ConsoleColor.Cyan, ConsoleColor.DarkCyan) },
        { PieceType.J, (ConsoleColor.Blue, ConsoleColor.DarkBlue) },
        { PieceType.L, (ConsoleColor.DarkYellow, ConsoleColor.Yellow) },
        { PieceType.O, (ConsoleColor.Yellow, ConsoleColor.DarkYellow) },
        { PieceType.S, (ConsoleColor.Green, ConsoleColor.DarkGreen) },
        { PieceType.T, (ConsoleColor.Magenta, ConsoleColor.DarkMagenta) },
        { PieceType.Z, (ConsoleColor.Red, ConsoleColor.DarkRed)}
        
    };
    
    private const string EmptyCell = "::";
    private const string FullCell = "██";
    private const string PreviewCell = "░░";
    private const string FlashCell = "[]";
    private const ConsoleColor DefaultColor = ConsoleColor.DarkGray;
    const int CellWidth = 2;
    public void Init()
    {
        Console.CursorVisible = false;
        Console.Title = "Tetris";
        Console.Clear();
        Console.ForegroundColor = DefaultColor;
    }

    private void DrawBoard(Board board)
    {
        for (int y = 0; y < board.Height; y++)
        {
            for (int x = 0; x < board.Width; x++)
            {
                Console.SetCursorPosition((x+BoardColumn)*CellWidth, y+BoardRow);
                if (board.IsCellFilled(x,y))
                {
                    Console.ForegroundColor = ColourMap[board.GetCellPieceType(x,y)].Item1;
                    Console.Write(FullCell);
                    Console.ForegroundColor = DefaultColor;
                }
                else
                {
                    Console.Write(EmptyCell);
                }
            }            
        }
        
        // draw borders
        for (int y = 0; y < board.Height; y++)
        {
            Console.SetCursorPosition((BoardColumn-1)*CellWidth, y+BoardRow);
            Console.Write(" ║");
            Console.SetCursorPosition((BoardColumn + board.Width)*CellWidth, y+BoardRow);
            Console.Write("║ ");
        }
        for (int x = 0; x < board.Width + 2; x++)
        {
            Console.SetCursorPosition((BoardColumn-1 + x)*CellWidth, BoardRow-1);
            string top = x == 0 ? " ╔" : x == board.Width+1 ? "╗ " : "══";
            Console.Write(top);
            Console.SetCursorPosition((BoardColumn-1 + x)*CellWidth, BoardRow+board.Height);
            string bottom = x == 0 ? " ╚" : x == board.Width+1 ? "╝ " : "══";
            Console.Write(bottom);
        }
    }
    
    public void Draw(Board board, ActivePiece activePiece, int points, GamePiece nextPiece, 
        ActivePiece dropPreviewPiece, GamePiece? storedPiece, List<int> clearingRows, bool flashOn)    
    {
        //draws board
        DrawBoard(board);
        
        //DRAW POINTS
        DrawText("Points:", new Cell(20,9));
        DrawText(points.ToString(),new Cell(20,10));
        
        //DRAW NEXT PIECE
        DrawText("Next Piece:", new Cell(20,14));
        ActivePiece tempNextPiece = new ActivePiece(nextPiece.PieceData, new Cell(20, 15), nextPiece.PieceType);
        ClearArea(20*CellWidth,15,4*CellWidth,5);
        DrawPiece(tempNextPiece.Cells(), tempNextPiece.PieceType);
        
        //DRAW STORED PIECE
        if (storedPiece != null)
        {
            DrawText("Stored Piece: ", new Cell(20, 1));
            ClearArea(20*CellWidth,2,4*CellWidth,5);
            ActivePiece tempStoredPiece = new ActivePiece(storedPiece.PieceData, new Cell(20, 2), storedPiece.PieceType);
            DrawPiece(tempStoredPiece.Cells(), tempStoredPiece.PieceType);
        }

        if (clearingRows.Count > 0 && flashOn)
        {
            FlashRows(clearingRows, board);
            return;
        }
        
        //DRAW DROP PREVIEW
        DrawPiece(dropPreviewPiece.Cells(), dropPreviewPiece.PieceType, true);
        
        //DRAW ACTIVE PIECE
        DrawPiece(activePiece.Cells(), activePiece.PieceType);
    }

    private void FlashRows(List<int> rowsToClear, Board board)
    {
        for (int r = 0; r < rowsToClear.Count; r++)
        {
            for (int x = 0; x < board.Width; x++)
            {
                Console.SetCursorPosition((x+BoardColumn)*CellWidth, rowsToClear[r]+BoardRow);
                Console.ForegroundColor = ColourMap[board.GetCellPieceType(x,rowsToClear[r])].Item1;
                Console.Write(FlashCell);
                Console.ForegroundColor = DefaultColor;
            }            
        }
    }

    private void ClearArea(int x, int y, int width, int height)
    {
        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                Console.SetCursorPosition(x + w, y + h);
                Console.Write(" ");
            }
        }
    }

    private void DrawText(string text, Cell location)
    {
        ClearArea(location.X*CellWidth, location.Y, text.Length, 1);
        Console.SetCursorPosition(location.X*CellWidth,location.Y);
        Console.Write(text);
    }

    private void DrawPiece(List<Cell> pieceCells, PieceType type, bool preview = false)
    {
        foreach (Cell coord in pieceCells)
        {
            Console.SetCursorPosition((coord.X + BoardColumn) * CellWidth , coord.Y+BoardRow);
            if (preview)
            {
                Console.ForegroundColor = ColourMap[type].Item2;
                Console.Write(PreviewCell);
            }
            else
            {
                Console.ForegroundColor = ColourMap[type].Item1;
                Console.Write(FullCell);
            }
            Console.ForegroundColor = DefaultColor;
        }
    }
}