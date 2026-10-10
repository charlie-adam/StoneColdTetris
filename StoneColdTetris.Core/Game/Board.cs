namespace StoneColdTetris.Core.Game;

public class Board
{
    private List<PieceType?[]> _grid = new List<PieceType?[]>();
    public readonly int Width = 10;
    public readonly int Height = 20;
    
    private PieceType?[] FreshRow()
    {
        PieceType?[] row = new PieceType?[Width];
        for (var fillW = 0; fillW < Width; fillW++)
        {
            row[fillW] = null;
        }
        return row;
    }

    public Board()
    {
        Reset();
    }

    public void Reset()
    {
        _grid.Clear();
        for (int fillH = 0; fillH < Height; fillH++)
        {
            _grid.Add(FreshRow());
        }
    }

    public bool IsCellFilled(int x, int y)
    {
        return _grid[y][x] != null;
    }

    public PieceType GetCellPieceType(int x, int y)
    {
        return _grid[y][x].Value;
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
            if (_grid[coord.Y][coord.X] != null)
            {
                return true;
            }
        }

        return false;
    }

    public void FillCells(List<Cell> pieceCells, PieceType pieceType)
    {
        foreach (Cell coord in pieceCells)
        {
            _grid[coord.Y][coord.X] = pieceType;
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
                if (_grid[row][col] != null)
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