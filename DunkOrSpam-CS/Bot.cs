using System.Security.Authentication;
using DunkOrSpam_CS.message;
using DunkOrSpam_CS.scheduler;
using DunkOrSpam_CS.utils;
using Newtonsoft.Json;
using WebSocketSharp;
using Logger = DunkOrSpam_CS.utils.Logger;

// using WebSocket = WebSocketSharp.WebSocket;
// Disabling warning because it's only relevant after socket closes, but ideally by then the program should be stopped
// ReSharper disable AccessToDisposedClosure

namespace DunkOrSpam_CS;

public class Bot(int hwnd) {

	private const string Uri = "wss://irc-ws.chat.twitch.tv:443";

	private readonly Logger logger = Program.Logger!;
	private readonly Scheduler scheduler = Scheduler.Instance;
	private readonly ManualResetEvent lockTite = new(false);
	private readonly Random random = new();

	private int hwnd = hwnd;

	private Options options;
	private WebSocket? ws;

	private bool messageQueued;
	private int lastIndex;

	public void LoadConfig(string file) {
		string contents = File.ReadAllText(file);
		
		options = JsonConvert.DeserializeObject<Options>(contents);
		Console.WriteLine(options.Nick);
		Console.WriteLine(options.Channel);
	}
	
	public void Connect() {
		 ws = new WebSocket(Uri);
		 ws.SslConfiguration.EnabledSslProtocols = SslProtocols.Tls12;
		 
		 ws.Connect();

		 Console.WriteLine("Socket opened");
		 
		 ws.Send($"PASS {options.OAuth}");
		 ws.Send($"NICK {options.Nick}");
		 ws.Send($"JOIN #{options.Channel}");
		 ws.Send("CAP REQ :twitch.tv/membership twitch.tv/tags twitch.tv/commands twitch.tv/foo");

		 ws.OnMessage += (_, args) => {
			 foreach (var message in args.Data.Trim().Split('\n')) {
				 OnMessage(Message.From(message));
			 }
		 };
		 
		 ws.OnClose += OnClose;
		 
		 lockTite.WaitOne();
	}

	public void SendMessage(string text) {
		if (ws!.IsAlive) {
			ws.Send(text);
		}
	}

	private void OnMessage(Message msg) {
		LogColor prefix = LogColor.Reset;

		switch (msg.Type) {
			case MessageType.PING:
				HandlePing(msg);
				return;
			case MessageType.PRIVMSG:
				HandlePrivateMessage(msg, out prefix);
				break;
			case MessageType.USERNOTICE:
				HandleUserNotice(msg, out prefix);
				break;
			case MessageType.NONE:
				logger.Error($"Message with unknown type: {msg.Body}");
				return;
			case MessageType.NUM:
				break;
			default:
				return;
		}

		logger.Log($"{LogColor.FgGreen}{msg.Author}{LogColor.Reset}: {prefix}{msg.Body}");
	}

	private void OnClose(object? sender, CloseEventArgs args) {
		lockTite.Set();
	}

	private void HandlePing(Message msg) {
		ws!.Send($"PONG {msg.Body}");
	}

	private void HandlePrivateMessage(Message msg, out LogColor prefix) {
		prefix = LogColor.Reset;

		// Disconnect if channel changes (Such as from a raid)
		if (msg.Channel != options.Channel) {
			logger.Warn("Channel changed. Shutting down");

			ws!.Send($"PART #{msg.Channel}");
			ws.Close(CloseStatusCode.Normal);
			
			return;
		}
		
		if (msg.Author != "dunkbot" && msg.Body.Contains(options.Nick, StringComparison.OrdinalIgnoreCase)) {
			prefix = LogColor.BgYellow;
			WindowUtils.FlashWindow(hwnd, 150, 10);
		} else if (msg.Author == options.Channel && msg.Body.StartsWith("!open", StringComparison.OrdinalIgnoreCase)) {
			prefix = LogColor.BgYellow;
		}
		
	}

	private void HandleUserNotice(Message msg, out LogColor prefix) {
		prefix = LogColor.Reset;

		if (msg.Tags["msg-id"].Contains("sub")) {
			logger.Log("New subscriber message found, queueing message...");
			prefix = LogColor.BgCyan;
			
			QueueMessage();
		}
	}

	private void QueueMessage() {
		if (messageQueued) return;

		int delay = random.Next(options.Minimum, options.Maximum + 1);

		logger.Log($"Queued message with {delay} seconds of delay");
		messageQueued = true;

		scheduler.ScheduleSync(() => {
			SendMessage($"PRIVMSG #{options.Channel} :{options.Messages[lastIndex]}");

			lastIndex = lastIndex + 1 > options.Messages.Length ? lastIndex + 1 : 0;
			messageQueued = false;
		}, delay * 50);
	}

}
