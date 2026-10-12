namespace StoneColdTetris.Core.Game;

public class Game
{
    public Board Board { get; } = new Board();
    public ActivePiece ActivePiece { get; private set; }
    public int Points { get; private set; }
    private int LinesCleared { get; set; }
    public int Level => 1 + LinesCleared / 10;
    public GamePiece? StoredPiece { get; private set; }
    
    private bool IsClearing => _clearingRows.Count > 0;

    private float _gravityTimer;
    private float GravityTick => MathF.Pow(0.8f - (Level - 1) * 0.007f, Level - 1);
    private float _flashTimer;
    private readonly float _flashDuration = 0.5f;
    private float _preLockTimer;
    private readonly float _preLockDuration = 0.5f;
    private List<int> _clearingRows = new List<int>();
    private List<GamePiece> _activeBag = new List<GamePiece>();
    private List<GamePiece> _nextBag = new List<GamePiece>();
    private bool _hasSwapped = false;
    private int _movesMadeSincePreLockTimerBegan = 0;

    public Game()
    {
        _activeBag = NewBag();
        _nextBag = NewBag();
        ActivePiece = NewPiece();
    }

    public void Update(float deltaTime)
    {
        if (_clearingRows.Count != 0)
        {   
            _flashTimer += deltaTime;
            if (_flashTimer > _flashDuration)
            {
                _flashTimer = 0;
                ScoreRows(_clearingRows.Count);
                LinesCleared += _clearingRows.Count;
                Board.ClearRows(_clearingRows);
                _clearingRows = new List<int>();
                FinishTurn();
            }
            return;
        }
        if (ActivePieceAtBottom())
        {
            _preLockTimer += deltaTime;
            if (_preLockTimer > _preLockDuration)
            {
                EndTurn();
                return;
            }
        }
        else
        {
            _preLockTimer = 0;
        }
        _gravityTimer += deltaTime;
        while (_gravityTimer > GravityTick)
        {
            _gravityTimer -= GravityTick;
            if (!MoveDown())
            {
                _gravityTimer = 0;
                break;
            }
        }
    }

    private bool ActivePieceAtBottom()
    {
        return Board.WillCollide(ActivePiece.Moved(0, 1).Cells());
    }

    private void IncrementMovesMadeSincePreLockBegan()
    {
        if (_preLockTimer > 0)
        {
            _movesMadeSincePreLockTimerBegan++;
        }
    }

    public bool FlashOn()
    {
        return (int)(_flashTimer / 0.1f) % 2 == 0;
    }

    public void MoveLeft()
    {
        if (IsClearing) return;
        if (TryApply(ActivePiece.Moved(-1, 0)))
        {
            IncrementMovesMadeSincePreLockBegan();
            if (_movesMadeSincePreLockTimerBegan < 15) _preLockTimer = 0;
        }
    }

    public void MoveRight()
    {
        if (IsClearing) return;
        if (TryApply(ActivePiece.Moved(1, 0)))
        {
            IncrementMovesMadeSincePreLockBegan();
            if (_movesMadeSincePreLockTimerBegan < 15) _preLockTimer = 0;
        }
    }

    public void Rotate()
    {
        if (IsClearing) return;
        if (TryApply(ActivePiece.Rotated()))
        {
            IncrementMovesMadeSincePreLockBegan();
            if (_movesMadeSincePreLockTimerBegan < 15) _preLockTimer = 0;
        }
        else
        {
            //try right to wall push
            if (TryApply(ActivePiece.Rotated().Moved(1, 0)))
            {
                IncrementMovesMadeSincePreLockBegan();
                if (_movesMadeSincePreLockTimerBegan < 15) _preLockTimer = 0;
            }
            else
            {
                //try left to wall push
                if (TryApply(ActivePiece.Rotated().Moved(-1, 0)))
                {
                    IncrementMovesMadeSincePreLockBegan();
                    if (_movesMadeSincePreLockTimerBegan < 15) _preLockTimer = 0;
                }
            }
        }
    }

    public void SoftDrop()
    {
        if (IsClearing) return;
        if (MoveDown())
        {
            Points++;
        }
    }

    public void HardDrop()
    {
        if (IsClearing) return;
        var rowsDropped = 0;
        while (TryApply(ActivePiece.Moved(0, 1)))
        {
            rowsDropped++;
        }
        Points += rowsDropped * 2;
        EndTurn();
    }

    public List<int> ClearingRows()
    {
        return _clearingRows;
    }

    private bool MoveDown()
    {
        if (IsClearing) return false;
        if (TryApply(ActivePiece.Moved(0, 1)))
        {
            _preLockTimer = 0;
            return true;
            
        }
        return false;
    }

    private void EndTurn()
    {
        LockPiece();
        List<int> rowsToClear = Board.FindFullRows();
        if (rowsToClear.Count > 0)
        {
            _clearingRows = rowsToClear;
            _flashTimer = 0;
            return;
        }
        FinishTurn();
    }

    private void FinishTurn()
    {
        ActivePiece = NewPiece();
        if (!TryApply(ActivePiece))
        {
            GameOver();
        }
    }

    private bool TryApply(ActivePiece candidate)
    {
        if (Board.WillCollide(candidate.Cells()))
            return false;
        ActivePiece = candidate;
        return true;
    }

    private void GameOver()
    {
        Points = 0;
        Board.Reset();
        _gravityTimer = 0;
        LinesCleared = 0;
        ActivePiece = NewPiece();
    }

    private void LockPiece()
    {
        //piece has hit bottom of possible fall, lock where they are
        Board.FillCells(ActivePiece.Cells(), ActivePiece.PieceType);
    }

    private void ScoreRows(int rowsCleared)
    {
        switch (rowsCleared)
        {
            case 1 :
                Points += 100 * Level;
                break;
            case 2 :
                Points += 300 * Level;
                break;
            case 3:
                Points += 500 * Level;
                break;
            case 4:
                Points += 800 * Level;
                break;
            default:
                break;
        }
    }

    public GamePiece NextPiece()
    {
        return _activeBag[^1];
    }

    public ActivePiece DropPreviewPiece()
    {
        //create temp piece and move down until 
        ActivePiece ghost = ActivePiece;
        while (!Board.WillCollide(ghost.Moved(0,1).Cells()))
        {
            ghost = ghost.Moved(0, 1);
        }

        return ghost;
    }

    private List<GamePiece> NewBag()
    {
        GamePiece[] bag = GamePiece.All.ToArray();
        Random.Shared.Shuffle(bag);
        return new List<GamePiece>(bag);
    }

    private void UpdateBags()
    {
        if (_nextBag.Count == 0)
            _nextBag = NewBag();
        if (_activeBag.Count == 0)
        {
            _activeBag = _nextBag;
            _nextBag = NewBag();
        }
    }

    private Cell GetFreshDropPosition()
    {
        int col = Board.Width / 2 - 1;
        int row = 0;
        return new Cell(col, row);
    }

    private ActivePiece NewPiece()
    {
        GamePiece randomPiece = SelectRandomPiece();
        _hasSwapped = false;
        _movesMadeSincePreLockTimerBegan = 0;
        return new ActivePiece(randomPiece.PieceData, GetFreshDropPosition(), randomPiece.PieceType);
    }

    private GamePiece SelectRandomPiece()
    {
        GamePiece piece = _activeBag[^1];
        _activeBag.RemoveAt(_activeBag.Count - 1);
        UpdateBags();
        return piece;
    }

    public void Store()
    {
        if (_hasSwapped)
        {
            return;
        }
        ActivePiece preSwapActivePiece = ActivePiece;
        if (StoredPiece != null)
        {
            ActivePiece = new ActivePiece(StoredPiece.PieceData, GetFreshDropPosition(), StoredPiece.PieceType);
        }
        else
        {
            ActivePiece = NewPiece();
        }

        _hasSwapped = true;
        StoredPiece = GamePiece.All.First(x => x.PieceType == preSwapActivePiece.PieceType);
    }
}
