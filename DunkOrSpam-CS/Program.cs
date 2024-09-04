using DunkOrSpam_CS.utils;

namespace DunkOrSpam_CS;

public static class Program {

	private const string Title = "DunkOrSpam-CS";

	public static Logger? Logger { get; private set; }
	
	// Literally just an entry point
	public static void Main() {
		Console.Title = Title;
		ConsoleUtils.EnableColorFormat();
		
		int hwnd = 0;

		// Certified jank to grab window title because it doesn't always update immediately
		// Setting Console.Title seems to update the actual window title asynchronously
		while (hwnd <= 0) {
			hwnd = WindowUtils.FindWindow(Title).ToInt32();
		}

		Logger = new Logger(hwnd);

		Bot bot = new(hwnd);
		
		bot.LoadConfig("./config.json");
		bot.Connect();
	}
	
}
