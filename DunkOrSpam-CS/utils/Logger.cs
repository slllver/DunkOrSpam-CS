using static DunkOrSpam_CS.utils.LogColor;

namespace DunkOrSpam_CS.utils;

public class Logger(int hwnd) {

	private static readonly object LogLock = new();

	private int hwnd = hwnd;
	
	private readonly List<string> messageQueue = [];
	private int lastPos;

	/// <summary>
	/// Logs the given text to stdout
	/// Log output can be colored with the format ^[b|f]DDm where DD is a number from 0-15
	/// </summary>
	/// <param name="text"></param>
	public void Log(string text) {
		Spool($"{FgGray}{Timestamp()} {Reset}[INFO] {text}{Reset}");
	}

	public void Warn(string text) {
		Spool($"{FgGray}{Timestamp()} {FgYellow}[WARN] {text}{Reset}");
	}

	public void Error(string text) {
		Spool($"{FgGray}{Timestamp()} {FgRed}[ERROR] {text}{Reset}");
	}

	private void Spool(string text) {
		lock (LogLock) {
			int currPos = WindowUtils.GetScroll(hwnd);

			if (currPos < lastPos) {
				QueueMessage(text);
				return;
			} else if (messageQueue.Count > 0) {
				PrintQueue();
			} else {
				Console.WriteLine(text);
			}
			
			lastPos = WindowUtils.GetScroll(hwnd);
		}
	}

	private void QueueMessage(string text) {
		messageQueue.Add(text);
	}

	private void PrintQueue() {
		foreach (var item in messageQueue) {
			Console.WriteLine(item);
		}
		
		messageQueue.Clear();
	}

	private void WriteLog(string text) {
		// TODO: Log to stdout and write logs to file. Also figure out how to write to file in realtime
	}

	private string Timestamp() {
		return DateTime.Now.ToString("hh:mm:ss tt");
	}

}
