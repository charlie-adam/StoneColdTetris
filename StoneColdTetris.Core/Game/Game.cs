namespace StoneColdTetris.Core.Game;

public class Game
{
    public Board Board { get; } = new Board();
    public ActivePiece ActivePiece { get; private set; }
    public int Points { get; private set; }
    public int Level { get; private set; }
    public GamePiece? StoredPiece { get; private set; }
    
    private bool IsClearing => _clearingRows.Count > 0;

    private float _gravityTimer;
    private float _gravityTick = 0.75f;
    private float _flashTimer;
    private float _flashDuration = 1f;
    private List<int> _clearingRows = new List<int>();
    private List<GamePiece> _activeBag = new List<GamePiece>();
    private List<GamePiece> _nextBag = new List<GamePiece>();
    private bool _hasSwapped = false;

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
                Board.ClearRows(_clearingRows);
                _clearingRows = new List<int>();
                FinishTurn();
            }
            return;
        }
        _gravityTimer += deltaTime;
        if (_gravityTimer > _gravityTick)
        {
            _gravityTimer -= _gravityTick;
            MoveDown();
        }
    }

    public bool FlashOn()
    {
        return (int)(_flashTimer / 0.1f) % 2 == 0;
    }

    public void MoveLeft()
    {
        if (IsClearing) return;
        TryApply(ActivePiece.Moved(-1, 0));
    }

    public void MoveRight()
    {
        if (IsClearing) return;
        TryApply(ActivePiece.Moved(1, 0));
    }

    public void Rotate()
    {
        if (IsClearing) return;
        TryApply(ActivePiece.Rotated());
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
            return true;

        //failed to move down further, must have collided!!
        EndTurn();
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
                Points += 40 * (Level + 1);
                break;
            case 2 :
                Points += 100 * (Level + 1);
                break;
            case 3:
                Points += 300 * (Level + 1);
                break;
            case 4:
                Points += 1200 * (Level + 1);
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
