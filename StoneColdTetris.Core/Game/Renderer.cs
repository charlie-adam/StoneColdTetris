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
    
    public void Draw(Board board, ActivePiece activePiece, int points)    
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

        foreach (Cell coord in activePiece.Cells())
        {
            Console.SetCursorPosition(coord.X * CellWidth , coord.Y);
            Console.ForegroundColor = ColourMap[activePiece.PieceType];
            Console.Write(FullCell);
            Console.ForegroundColor = DefaultColor;
        }
        
        Console.SetCursorPosition(40,9);
        Console.Write("Points:");
        
        Console.SetCursorPosition(40,10);
        Console.Write(points.ToString().PadRight(10));
    }
}