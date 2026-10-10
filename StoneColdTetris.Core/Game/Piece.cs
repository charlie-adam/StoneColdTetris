namespace StoneColdTetris.Core.Game;

public class GamePiece
{
    public PieceType PieceType;
    public int[,] PieceData;
    public GamePiece(PieceType type, int[,] data)
    {
        PieceType = type;
        PieceData = data;
    }
    
    public static readonly IReadOnlyList<GamePiece> All = new List<GamePiece>()
    {
        new GamePiece(PieceType.L, new int[,] {
            { 0, 1, 0 },
            { 0, 1, 0 },
            { 0, 1, 1 }
        }),
    
        new GamePiece(PieceType.S, new int[,] {
            { 0, 0, 0 },
            { 0, 1, 1 },
            { 1, 1, 0 }
        }),
    
        new GamePiece(PieceType.T, new int[,] {
            { 0, 0, 0 },
            { 1, 1, 1 },
            { 0, 1, 0 }
        }),
    
        new GamePiece(PieceType.Z, new int[,] {
            { 0, 0, 0 },
            { 1, 1, 0 },
            { 0, 1, 1 }
        }),
    
        new GamePiece(PieceType.J, new int[,] {
            { 0, 1, 0 },
            { 0, 1, 0 },
            { 1, 1, 0 }
        }),
    
        new GamePiece(PieceType.I, new int[,] {
            { 0, 1, 0, 0 },
            { 0, 1, 0, 0 },
            { 0, 1, 0, 0 },
            { 0, 1, 0, 0 }
        }),
    
        new GamePiece(PieceType.O, new int[,] {
            { 1, 1 },
            { 1, 1 }
        })
    };
}
public record struct Cell(int X, int Y);

public class ActivePiece
{
    private readonly int[,] _pieceData;
    private readonly Cell _position;
    public readonly PieceType PieceType;
    public ActivePiece(int[,] pieceData, Cell position, PieceType pieceType)
    {
        _pieceData = pieceData;
        _position = position;
        PieceType = pieceType;
    }

    public List<Cell> Cells()
    {
        List<Cell> coordinatesOfActivePieceCells = new List<Cell>();

        for (int row = 0; row < _pieceData.GetLength(0);  row++)
        {
            for (int column = 0; column < _pieceData.GetLength(1); column++)
            {
                if (_pieceData[row, column] == 1)
                {
                    coordinatesOfActivePieceCells.Add(new Cell(_position.X + column, _position.Y + row));
                }
            }
        }
        return coordinatesOfActivePieceCells;
    }
    
    public ActivePiece Moved(int dx, int dy)
    {
        return new ActivePiece(_pieceData, new Cell(_position.X + dx, _position.Y + dy), PieceType);
    }
    
    public ActivePiece Rotated()
    {
        //rotate clockwise, matricies transform & reverse.
        int pieceLength = _pieceData.GetLength(0);
        int pieceWidth = _pieceData.GetLength(1);
        int[,] transposed = new int[pieceLength, pieceWidth];
        for (int r = 0; r < pieceLength; r++)
        {
            for (int c = 0; c < pieceLength; c++)
            {
                transposed[c, r] = _pieceData[r, c];
            }
        }
        
        //reverse
        int[,] rotated = new int[pieceLength, pieceWidth];
        for (int r = 0; r < pieceLength; r++)
        {
            for (int c = 0; c < pieceLength; c++)
            {
                rotated[r, c] = transposed[r, pieceLength-1 - c];
            }
        }

        return new ActivePiece(rotated, _position, PieceType);
    }
}

public enum PieceType
{
    L,
    S,
    T,
    Z,
    J,
    I,
    O
}