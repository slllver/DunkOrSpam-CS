namespace DunkOrSpam_CS;

public struct Options(string oAuth, string nick, string channel, int minimum, int maximum, string[] messages) {
	
	public string OAuth = oAuth;
	public string Nick = nick;
	public string Channel = channel;
	public int Minimum = minimum;
	public int Maximum = maximum;
	public string[] Messages = messages;

}
