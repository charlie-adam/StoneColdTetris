using System.Diagnostics;
namespace StoneColdTetris.Core.Game;

public class GameLoop
{
    public static void Run()
    {
        Game game = new Game();
        Renderer renderer = new Renderer();
        renderer.Init();
        Stopwatch stopwatch = Stopwatch.StartNew();
        long lastTime = stopwatch.ElapsedMilliseconds;
        bool isRunning = true;

        while (isRunning)
        {
            long currentTime = stopwatch.ElapsedMilliseconds;
            float deltaTime = (currentTime - lastTime) / 1000.0f;
            lastTime = currentTime;

            isRunning = ProcessInput(game);
            game.Update(deltaTime);
            renderer.Draw(game.Board, game.ActivePiece, game.Points);

            Thread.Sleep(16); // ~60 FPS cap
        }
    }

    // returns false when the player wants to quit
    static bool ProcessInput(Game game)
    {
        if (!Console.KeyAvailable)
            return true;

        switch (Console.ReadKey(true).Key)
        {
            case ConsoleKey.Escape:
                return false;
            case ConsoleKey.LeftArrow:
                game.MoveLeft();
                break;
            case ConsoleKey.RightArrow:
                game.MoveRight();
                break;
            case ConsoleKey.UpArrow:
                game.Rotate();
                break;
            case ConsoleKey.DownArrow:
                game.SoftDrop();
                break;
        }
        return true;
    }
}