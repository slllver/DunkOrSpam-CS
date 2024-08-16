using DunkOrSpam_CS.utils;

namespace DunkOrSpam_CS;

public static class Program {

	private const string Title = "DunkOrSpam";

	public static Logger? Logger { get; private set; }
	
	// Literally just an entry point
	public static void Main() {
		Console.Title = Title;
		ConsoleUtils.EnableColorFormat();
		Logger = new Logger(WindowUtils.FindWindow(Title).ToInt32());

		// Bot bot = new();
		//
		// bot.LoadConfig("./config.json");
		// bot.Connect();

		int i = WindowUtils.FindWindow(Title).ToInt32();

		// TODO: IT'S A FUCKING RACE CONDITION. i'm going to bed.
		Console.WriteLine(WindowUtils.GetScroll(i));
		Console.WriteLine(WindowUtils.GetScroll(WindowUtils.FindWindow(Title)));

		Console.ReadKey();
	}
	
}
