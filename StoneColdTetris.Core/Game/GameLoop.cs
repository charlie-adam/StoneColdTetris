using System.Diagnostics;
namespace StoneColdTetris.Core.Game;

public class GameLoop
{
    private static bool _isRunning = true;
    private static float _gravityTimer = 0.00f;
    private static float _gravityTick = 0.75f;
    private static int _points = 0;
    private static int _level = 0;
    const int CellWidth = 2;
    private static ActivePiece _activePiece;
    private static ConsoleKey? _activeKey = null;
    private const string emptyKey = "..";
    private const string fullKey = "XX";
    private static Board _board = new Board();

    public static void Run()
    {
        Console.CursorVisible = false;
        Stopwatch stopwatch = Stopwatch.StartNew();
        long lastTime = stopwatch.ElapsedMilliseconds;
        
        Console.Title = "Tetris";
        AddNewPiece();

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
        ActivePiece candidate = _activePiece.Rotated();
        if (_board.WillCollide(candidate.Cells()))
            return false;
        _activePiece = candidate;
        return true;
    }

    static bool TryMoveDown()
    {
        ActivePiece candidate = _activePiece.Moved(0,1);
        if (_board.WillCollide(candidate.Cells()))
        {
            LockPiece();
            CheckRows();
            AddNewPiece();
            return false;
        }

        _activePiece = candidate;
        return true;
    }

    static bool TryMoveLeft()
    {
        ActivePiece candidate = _activePiece.Moved(-1,0);
        if (_board.WillCollide(candidate.Cells()))
            return false;
        _activePiece = candidate;
        return true;
    }

    static bool TryMoveRight()
    {
        ActivePiece candidate = _activePiece.Moved(1,0);
        if (_board.WillCollide(candidate.Cells()))
            return false;
        _activePiece = candidate;
        return true;
    }
    
    static GamePiece SelectRandomPiece()
    {
        int randomValue = Random.Shared.Next(0, 7);
        return GamePiece.All[randomValue];
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
        int rowsCleared = _board.ClearRows();
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
        List<Cell> activePieceCells = _activePiece.Cells();
        _board.FillCells(activePieceCells);
    }

    static void AddNewPiece()
    {
        int col = _board.Width / 2 - 1;
        int row = 0;
        _activePiece = new ActivePiece(SelectRandomPiece().PieceData, new Cell(col,row));
    }

    static void Render()
    {
        //draws board
        for (int h = 0; h < _board.Height; h++)
        {
            for (int l = 0; l < _board.Width; l++)
            {
                Console.SetCursorPosition(l * CellWidth, h);
                if (_board.IsCellFilled(l,h))
                {
                    Console.Write(fullKey);
                }
                else
                {
                    Console.Write(emptyKey);
                }
            }            
        }

        foreach (Cell coord in _activePiece.Cells())
        {
            Console.SetCursorPosition(coord.X * CellWidth , coord.Y);
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
}