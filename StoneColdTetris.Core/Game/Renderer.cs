namespace StoneColdTetris.Core.Game;

public class Renderer
{
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
    
    private const string EmptyCell = "..";
    private const string FullCell = "██";
    private const ConsoleColor DefaultColor = ConsoleColor.DarkGray;
    const int CellWidth = 2;
    public void Init()
    {
        Console.CursorVisible = false;
        Console.Title = "Tetris";
        Console.Clear();
        Console.ForegroundColor = DefaultColor;
    }
    
    public void Draw(Board board, ActivePiece activePiece, int points, GamePiece nextPiece)    
    {
        //draws board
        for (int y = 0; y < board.Height; y++)
        {
            for (int x = 0; x < board.Width; x++)
            {
                Console.SetCursorPosition(x* CellWidth, y);
                if (board.IsCellFilled(x,y))
                {
                    Console.ForegroundColor = ColourMap[board.GetCellPieceType(x,y)];
                    Console.Write(FullCell);
                    Console.ForegroundColor = DefaultColor;
                }
                else
                {
                    Console.Write(EmptyCell);
                }
            }            
        }
        
        DrawPiece(activePiece.Cells(), activePiece.PieceType);
        
        //DRAW POINTS
        
        Console.SetCursorPosition(40,9);
        Console.Write("Points:");
        
        Console.SetCursorPosition(40,10);
        Console.Write(points.ToString().PadRight(10));
        
        //DRAW NEXT PIECE
        ActivePiece tempNextPiece = new ActivePiece(nextPiece.PieceData, new Cell(20, 12), nextPiece.PieceType);
        ClearArea(20*CellWidth,12,4*CellWidth,4);
        DrawPiece(tempNextPiece.Cells(), nextPiece.PieceType);
    }

    private void ClearArea(int x, int y, int width, int height)
    {
        for (int w = 0; w < width; w++)
        {
            for (int h = 0; h < height; h++)
            {
                Console.SetCursorPosition(x + w, y + h);
                Console.Write(".");
            }
        }
    }

    private void DrawPiece(List<Cell> pieceCells, PieceType type)
    {
        foreach (Cell coord in pieceCells)
        {
            Console.SetCursorPosition(coord.X * CellWidth , coord.Y);
            Console.ForegroundColor = ColourMap[type];
            Console.Write(FullCell);
            Console.ForegroundColor = DefaultColor;
        }
    }
}