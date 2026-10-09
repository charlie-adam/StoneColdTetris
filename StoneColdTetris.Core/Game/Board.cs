namespace StoneColdTetris.Core.Game;

public class Board
{
    private List<int[]> _grid = new List<int[]>();
    public int Width = 10;
    public int Height = 20;
    
    private int[] FreshRow()
    {
        int[] row = new int[Width];
        for (int fillW = 0; fillW < Width; fillW++)
        {
            row[fillW] = 0;
        }
        return row;
    }

    public Board()
    {
        for (int fillH = 0; fillH < Height; fillH++)
        {
            _grid.Add(FreshRow());
        }
    }

    public bool IsCellFilled(int row, int column)
    {
        return _grid[row][column] == 0;
    }
    
    public bool WillCollide(List<Cell> pieceCells)
    {
        foreach (Cell coord in pieceCells)
        {   
            if (
                (coord.X >= Width) ||
                (coord.X < 0) || 
                (coord.Y >= Height))
            {
                return true;
            }
            
            //also check if coord is going to collide with any filled parts of the board
            if (_grid[coord.Y][coord.X] == 1)
            {
                return true;
            }
        }

        return false;
    }

    public void FillCells(List<Cell> pieceCells)
    {
        foreach (Cell coord in pieceCells)
        {
            _grid[coord.Y][coord.X] = 1;
        }
    }

    public int ClearRows()
    {
        int rowsCleared = 0;
        for (int row = 0; row < Height; row++)
        {
            int total = 0;
            for (int col = 0; col < Width; col++)
            {
                if (_grid[row][col] == 1)
                {
                    total++;
                }
            }
            if (total == Width)
            {
                rowsCleared++;
                //shift rows down
                _grid.RemoveAt(row);
                _grid.Insert(0,FreshRow());
            }
        }

        return rowsCleared;
    }
}