using TestGtkApp;
using Gtk;

public static class Program
{
	static void Main()
	{
		Application.Init();
		MainWindow mainWindow = [];
		mainWindow.ShowAll();
		Application.Run();
	}
}
	