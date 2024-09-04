namespace DunkOrSpam_CS.message;

public class Message {

	public string Author;
	public MessageType Type;
	public string Channel;
	public string Body;
	public readonly Dictionary<string, string> Tags = new();

	private Message(string author = "Twitch", MessageType type = MessageType.NONE, string channel = "", string body = "") {
		Author = author;
		Type = type;
		Channel = channel;
		Body = body;
	}
	
	public static Message From(string text) {
		Message message = new();

		if (text.StartsWith("PING")) {
			message.Type = MessageType.PING;
			message.Body = text.Substring(5);
			
			return message;
		}

		int tagIndex = 0;
		
		if (text.StartsWith("@")) {
			var tags = message.Tags;
			
			text.Substring(0, tagIndex = text.IndexOf(' ')).Split(';').ToList().ForEach(arg => {
				string[] kv = arg.Split('=');
				tags[kv[0]] = kv[1];
			});

			if (tags.TryGetValue("display-name", out var name)) {
				message.Author = name;
			}
		}

		int commandStart = text.IndexOf(' ', tagIndex + 1) + 1;
		int commandEnd = text.IndexOf(':', commandStart) + 1;

		if (commandEnd == 0) commandEnd = text.Length;
		
		string[] excess = text.Substring(commandStart, commandEnd - commandStart).Split(' ');

		message.Type = GetType(excess[0]);
		message.Body = text.Substring(commandEnd);

		if (excess.Length >= 2) {
			message.Channel = excess[1].Trim('#');
		} 

		return message;
	}

	public override string ToString() {
		string tagString = Tags.Count > 0 ? $"\n\t\t{string.Join("\n\t\t", Tags)}\n\t" : "";
		
		return $"{{\n\tType: {Type}\n\tAuthor: {Author}\n\tChannel: {Channel}\n\tBody: {Body}\n\tTags: [{tagString}]\n}}";
	}

	private static MessageType GetType(string input) {
		MessageType output;
		
		if (int.TryParse(input, out _)) {
			output = MessageType.NUM;
		} else if (!Enum.TryParse(input, out output)) {
			output = MessageType.NUM;
		}

		return output;
	}

}
