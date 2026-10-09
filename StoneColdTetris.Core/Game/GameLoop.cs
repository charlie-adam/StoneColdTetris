using System.Diagnostics;
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
}
public record struct Cell(int X, int Y);

public class ActivePiece
{
    public int[,] PieceData;
    public Cell position;
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

public class GameLoop
{
    private static bool _isRunning = true;
    private static float _gravityTimer = 0.00f;
    private static float _gravityTick = 0.75f;
    private static int _points = 0;
    private static int _level = 0;
    private static List<GamePiece> _gamePieces = new List<GamePiece>
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
    private static ActivePiece _activePiece;
    private static List<int[]> _board = new List<int[]>();
    private static int _boardWidth = 10;
    private static int _boardHeight = 20;
    private static ConsoleKey? _activeKey = null;
    private const string emptyKey = "..";
    private const string fullKey = "XX";

    private const int _cellWidth = 2;
    

    public static void Run()
    {
        Console.CursorVisible = false;
        Stopwatch stopwatch = Stopwatch.StartNew();
        long lastTime = stopwatch.ElapsedMilliseconds;
        InitialiseBoard();
        Console.Title = "Tetris";
        AddPiece(SelectRandomPiece().PieceType);

        while (_isRunning)
        {
            long currentTime = stopwatch.ElapsedMilliseconds;
            float deltaTime = (currentTime - lastTime) / 1000.0f;
            lastTime = currentTime;

            ProcessInput();
            Update(deltaTime);
            Render();

            Thread.Sleep(16); // ~60 FPS cap
        }
    }

    static void ProcessInput()
    {
        if (Console.KeyAvailable)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            _activeKey = keyInfo.Key;
            if (keyInfo.Key == ConsoleKey.Escape)
            {
                Console.WriteLine("Escape key pressed");
                _isRunning = false;
            }

            switch (_activeKey)
            {
                case ConsoleKey.LeftArrow:
                    TryMoveLeft();
                    break;
                case ConsoleKey.RightArrow:
                    TryMoveRight();
                    break;
                case ConsoleKey.UpArrow:
                    TryRotate();
                    break;
                case ConsoleKey.DownArrow:
                    _points++;
                    TryMoveDown();
                    break;
            }
        }
    }

    static bool TryRotate()
    {
        //rotate clockwise, matricies transform & reverse.
        int pieceLength = _activePiece.PieceData.GetLength(0);
        int pieceWidth = _activePiece.PieceData.GetLength(1);
        int[,] transposed = new int[pieceLength, pieceWidth];
        for (int r = 0; r < pieceLength; r++)
        {
            for (int c = 0; c < pieceLength; c++)
            {
                transposed[c, r] = _activePiece.PieceData[r, c];
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
        
        if (willCollide(GetCoordinatesOfPieceCells(rotated)))
        {
            return false;
        }
        _activePiece.PieceData = rotated;
        return true;
    }

    static bool TryMoveDown()
    {
        if (willCollide(getCoordinatesOfActivePieceAfterTranslation(0,1)))
        {
            LockPiece();
            CheckRows();
            AddPiece(SelectRandomPiece().PieceType);
            return false;
        }
        _activePiece.position.Y++;
        return true;
    }

    static bool TryMoveLeft()
    {
        if (willCollide(getCoordinatesOfActivePieceAfterTranslation(-1,0)))
            return false;
        _activePiece.position.X--;
        return true;
    }

    static bool TryMoveRight()
    {
        if (willCollide(getCoordinatesOfActivePieceAfterTranslation(1,0)))
            return false;
        _activePiece.position.X++;
        return true;
    }
    
    static GamePiece SelectRandomPiece()
    {
        int randomValue = Random.Shared.Next(0, 7);
        return _gamePieces[randomValue];
    }
    

    static void Update(float deltaTime)
    {
        _gravityTimer += deltaTime;
        if (_gravityTimer > _gravityTick)
        {
            _gravityTimer -= _gravityTick;
            TryMoveDown();
        }
    }

    static void CheckRows()
    {
        int rowsCleared = 0;
        for (int row = 0; row < _boardHeight; row++)
        {
            int total = 0;
            for (int col = 0; col < _boardWidth; col++)
            {
                if (_board[row][col] == 1)
                {
                    total++;
                }
            }
            if (total == _boardWidth)
            {
                rowsCleared++;
                //shift rows down
                _board.RemoveAt(row);
                _board.Insert(0,FreshRow());
            }
        }

        switch (rowsCleared)
        {
            case 1 :
                _points += 40 * (_level+1);
                break;
            case 2 :
                _points += 100 * (_level + 1);
                break;
            case 3: 
                _points += 300 * (_level + 1);
                break;
            case 4:
                _points += 1200 * (_level + 1);
                break;
            default:
                break;
        }

    }

    static void LockPiece()
    {
        //piece has hit bottom of possible fall, lock where they are
        List<Cell> activePieceCells = GetCoordinatesOfPieceCells(_activePiece.PieceData);
        foreach (Cell coord in activePieceCells)
        {
            //walk the board
            // Console.SetCursorPosition(coord[0] * _cellWidth , coord[1]);
            // Console.Write(fullKey);
            _board[coord.Y][coord.X] = 1;
        }
    }

    static List<Cell> GetCoordinatesOfPieceCells(int[,] pieceData)
    {
        List<Cell> coordinatesOfActivePieceCells = new List<Cell>();

        for (int row = 0; row < pieceData.GetLength(0);  row++)
        {
            for (int column = 0; column < pieceData.GetLength(1); column++)
            {
                if (pieceData[row, column] == 1)
                {
                    coordinatesOfActivePieceCells.Add(new Cell(_activePiece.position.X + column, _activePiece.position.Y + row));
                }
            }
        }
        return coordinatesOfActivePieceCells;
    }

    static List<Cell> getCoordinatesOfActivePieceAfterTranslation(int moveX = 0, int moveY = 0)
    {
        List<Cell> activePieceCellCoordinates = GetCoordinatesOfPieceCells(_activePiece.PieceData);
        List<Cell> pieceCellCoordinatesAfterTranslation = new List<Cell>();
        foreach (Cell coord in activePieceCellCoordinates)
        {
            int xCoordWithOffset = coord.X + moveX;
            int yCoordWithOffset = coord.Y + moveY;
            pieceCellCoordinatesAfterTranslation.Add(new Cell(xCoordWithOffset, yCoordWithOffset));
        }

        return pieceCellCoordinatesAfterTranslation;
    }
    
    static bool willCollide(List<Cell> pieceCells)
    {
        foreach (Cell coord in pieceCells)
        {   
            if (
                (coord.X >= _boardWidth) ||
                (coord.X < 0) || 
                (coord.Y >= _boardHeight))
            {
                return true;
            }
            
            //also check if coord is going to collide with any filled parts of the board
            if (_board[coord.Y][coord.X] == 1)
            {
                return true;
            }
        }

        return false;
    }

    static void AddPiece(PieceType pieceType)
    {
        int col = _boardWidth / 2 - 1;
        int row = 0;
        GamePiece g = _gamePieces.Find(x => x.PieceType == pieceType);
        ActivePiece newActivePiece = new ActivePiece();
        newActivePiece.PieceData = g.PieceData;
        newActivePiece.position.X = col;
        newActivePiece.position.Y = row;
        _activePiece = newActivePiece;
    }

    static void Render()
    {
        //draws board
        for (int h = 0; h < _boardHeight; h++)
        {
            for (int l = 0; l < _boardWidth; l++)
            {
                Console.SetCursorPosition(l * _cellWidth, h);
                if (_board[h][l] == 0)
                {
                    Console.Write(emptyKey);
                }
                else
                {
                    Console.Write(fullKey);
                }
            }            
        }
        List<Cell> activePieceCells = GetCoordinatesOfPieceCells(_activePiece.PieceData);
        foreach (Cell coord in activePieceCells)
        {
            Console.SetCursorPosition(coord.X * _cellWidth , coord.Y);
            Console.Write(fullKey);
        }
        
        //draw key
        if (_activeKey != null)
        {
            Console.SetCursorPosition(60,10);
            Console.Write(_activeKey.Value.ToString());
        }

        Console.SetCursorPosition(40,9);
        Console.Write("Points:");
        
        Console.SetCursorPosition(40,10);
        Console.Write(_points.ToString());
    }

    static int[] FreshRow()
    {
        int[] row = new int[_boardWidth];
        for (int fillW = 0; fillW < _boardWidth; fillW++)
        {
            row[fillW] = 0;
        }

        return row;
    }

    static void InitialiseBoard()
    {
        for (int fillH = 0; fillH < _boardHeight; fillH++)
        {
            _board.Add(FreshRow());
        }
    }
}