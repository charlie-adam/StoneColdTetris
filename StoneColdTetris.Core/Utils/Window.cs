public static class WindowHelper
{
    // public static void Resize(int cols, int rows)
    // {
    //     Console.Write($"\x1b[8;{rows};{cols}t");
    // }

    public static void Clear()
    {
        Console.Clear();
    }
    
    public static void Begin()
    {
        Console.CursorVisible = false;
        // Resize(100, 50);
        Console.Clear();
    }
    public static void End()
    {}
}