using System.Security.Authentication;
using System.Text.RegularExpressions;
using DunkOrSpam_CS.scheduler;
using DunkOrSpam_CS.utils;
using Newtonsoft.Json;
using WebSocketSharp;
using Logger = DunkOrSpam_CS.utils.Logger;

// using WebSocket = WebSocketSharp.WebSocket;
// Disabling warning because it's only relevant after socket closes, but ideally by then the program should be stopped
// ReSharper disable AccessToDisposedClosure

namespace DunkOrSpam_CS;

public partial class Bot {

	private const string Uri = "wss://irc-ws.chat.twitch.tv:443";

	private readonly Logger logger = Program.Logger!;
	private readonly Scheduler scheduler = Scheduler.Instance;
	private readonly ManualResetEvent lockTite = new ManualResetEvent(false);
	private readonly Random random = new();

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

		 ws.OnMessage += OnMessage;
		 ws.OnClose += OnClose;
		 
		 lockTite.WaitOne();
	}

	private void OnMessage(object? sender, MessageEventArgs message) {
		Message msg = Message.From(message.Data.Trim());
	
		if (msg.Type == "PING") {
			ws!.Send($"PONG {msg.Body}");
			
			return;
		}
	
		// Disconnect if channel changes (Such as from a raid)
		if (msg.Channel != null && msg.Channel != options.Channel) {
			logger.Warn("Channel changed. Shutting down");
			
			ws!.Send($"PART #{msg.Channel}");
			ws.Close(CloseStatusCode.Normal);
		}
		
		if (msg.Author is null) {
			return;
		}

		LogColor highlight = LogColor.Reset;

		if (msg.Author is "wizebot" && msg.Body.Contains("\u2b50\ufe0f")) {
			logger.Log("New subscriber message found, queueing message...");
			QueueMessage();
		} else if (msg.Author != options.Nick && msg.Author != "dunkbot" && msg.Body.Contains(options.Nick, StringComparison.OrdinalIgnoreCase)) {
			highlight = LogColor.BgYellow;
			WindowUtils.FlashWindow(0, 150, 10);
		} else if (msg.Author == options.Channel && msg.Body.Contains("!open", StringComparison.OrdinalIgnoreCase)) {
			highlight = LogColor.BgYellow;
		}

		logger.Log($"{LogColor.FgGreen}{msg.Author}{LogColor.Reset}: {highlight}{msg.Body}");
	}

	private void OnClose(object? sender, CloseEventArgs args) {
		lockTite.Set();
	}

	private void QueueMessage() {
		if (messageQueued) return;

		int delay = random.Next(options.Minimum, options.Maximum + 1);

		logger.Log($"Queued message with {delay} seconds of delay");
		messageQueued = true;

		scheduler.ScheduleSync(() => {
			if (!ws!.IsAlive) return;
			
			ws.Send($"PRIVMSG #{options.Channel} :{options.Messages[lastIndex]}");
			lastIndex = lastIndex + 1 > options.Messages.Length ? lastIndex + 1 : 0;
			messageQueued = false;
		}, delay * 50);
	}

	private partial struct Message(string? author, string? type, string? channel, string body) {

		private const string MessagePattern =
			@":([a-z0-9_]{4,25})!\1@\1\.tmi\.twitch\.tv ([A-Z]+) #([a-z0-9_]{4,25}) :(.+)";

		public readonly string? Author = author;
		public readonly string? Type = type;
		public readonly string? Channel = channel;
		public readonly string Body = body;

		public static Message From(string text) {
			var match = MyRegex().Match(text);

			if (match is { Success: true, Groups.Count: 5 }) {
				var groups = match.Groups;
				return new Message(groups[1].ToString(), groups[2].ToString(), groups[3].ToString(), groups[4].ToString());
			}

			string? type = text.Contains("PING") ? "PING" : null;
			string body = type is null ? text : text[5..];

			return new Message(null, type, null, body);
		}

        [GeneratedRegex(MessagePattern)]
        private static partial Regex MyRegex();
    }

}
