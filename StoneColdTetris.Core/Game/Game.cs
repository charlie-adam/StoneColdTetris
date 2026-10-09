namespace StoneColdTetris.Core.Game;

public class Game
{
    public Board Board { get; } = new Board();
    public ActivePiece ActivePiece { get; private set; }
    public int Points { get; private set; }
    public int Level { get; private set; }

    private float _gravityTimer;
    private float _gravityTick = 0.75f;

    public Game()
    {
        ActivePiece = NewPiece();
    }

    public void Update(float deltaTime)
    {
        _gravityTimer += deltaTime;
        if (_gravityTimer > _gravityTick)
        {
            _gravityTimer -= _gravityTick;
            MoveDown();
        }
    }

    public void MoveLeft()
    {
        TryApply(ActivePiece.Moved(-1, 0));
    }

    public void MoveRight()
    {
        TryApply(ActivePiece.Moved(1, 0));
    }

    public void Rotate()
    {
        TryApply(ActivePiece.Rotated());
    }

    public void SoftDrop()
    {
        if (MoveDown())
        {
            Points++;
        }
    }

    public void HardDrop()
    {
        var rowsDropped = 0;
        while (TryApply(ActivePiece.Moved(0, 1)))
        {
            rowsDropped++;
        }
        Points += rowsDropped * 2;
        EndTurn();        
    }

    private bool MoveDown()
    {
        if (TryApply(ActivePiece.Moved(0, 1)))
            return true;

        //failed to move down further, must have collided!!
        EndTurn();
        return false;
    }

    private void EndTurn()
    {
        LockPiece();
        ScoreRows(Board.ClearRows());
        ActivePiece = NewPiece();
    }

    private bool TryApply(ActivePiece candidate)
    {
        if (Board.WillCollide(candidate.Cells()))
            return false;
        ActivePiece = candidate;
        return true;
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

    private ActivePiece NewPiece()
    {
        int col = Board.Width / 2 - 1;
        int row = 0;
        GamePiece randomPiece = SelectRandomPiece();
        return new ActivePiece(randomPiece.PieceData, new Cell(col, row), randomPiece.PieceType);
    }

    private GamePiece SelectRandomPiece()
    {
        int randomValue = Random.Shared.Next(GamePiece.All.Count);
        return GamePiece.All[randomValue];
    }
}
