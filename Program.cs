using Gtk;

class Program
{
    static void Main()
    {
        Application.Init();

        GameState game = new GameState();
        ChessWindow window = new ChessWindow(game);

        window.ShowAll();

        Application.Run();
    }
}