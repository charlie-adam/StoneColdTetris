namespace StoneColdTetris.Core.Game;

public class Renderer
{
    private const string _emptyCell = "..";
    private const string _fullCell = "XX";
    const int CellWidth = 2;

    public void Init()
    {
        Console.CursorVisible = false;
        Console.Title = "Tetris";
        Console.Clear();
    }
    
    public void Draw(Board board, ActivePiece piece, int points)    
    {
        //draws board
        for (int y = 0; y < board.Height; y++)
        {
            for (int x = 0; x < board.Width; x++)
            {
                Console.SetCursorPosition(x* CellWidth, y);
                if (board.IsCellFilled(x,y))
                {
                    Console.Write(_fullCell);
                }
                else
                {
                    Console.Write(_emptyCell);
                }
            }            
        }

        foreach (Cell coord in piece.Cells())
        {
            Console.SetCursorPosition(coord.X * CellWidth , coord.Y);
            Console.Write(_fullCell);
        }
        
        Console.SetCursorPosition(40,9);
        Console.Write("Points:");
        
        Console.SetCursorPosition(40,10);
        Console.Write(points.ToString());
    }
}