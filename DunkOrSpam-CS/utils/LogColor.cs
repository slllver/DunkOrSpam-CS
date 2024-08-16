namespace DunkOrSpam_CS.utils;

public class LogColor(string value) {
	
	public static LogColor Reset { get; private set; } = new("\\x1b[0m");
	public static LogColor Bright { get; private set; } = new("\\x1b[1m");
	public static LogColor Dim { get; private set; } = new("\\x1b[2m");
	public static LogColor Underscore { get; private set; } = new("\\x1b[4m");
	public static LogColor Blink { get; private set; } = new("\\x1b[5m");
	public static LogColor Reverse { get; private set; } = new("\\x1b[7m");
	public static LogColor Hidden { get; private set; } = new("\\x1b[8m");
	
	public static LogColor FgBlack { get; private set; } = new("\\x1b[30m");
	public static LogColor FgRed { get; private set; } = new("\\x1b[31m");
	public static LogColor FgGreen { get; private set; } = new("\\x1b[32m");
	public static LogColor FgYellow { get; private set; } = new("\\x1b[33m");
	public static LogColor FgBlue { get; private set; } = new("\\x1b[34m");
	public static LogColor FgMagenta { get; private set; } = new("\\x1b[35m");
	public static LogColor FgCyan { get; private set; } = new("\\x1b[36m");
	public static LogColor FgWhite { get; private set; } = new("\\x1b[37m");
	public static LogColor FgGray { get; private set; } = new("\\x1b[90m");
	
	public static LogColor BgBlack { get; private set; } = new("\\x1b[40m");
	public static LogColor BgRed { get; private set; } = new("\\x1b[41m");
	public static LogColor BgGreen { get; private set; } = new("\\x1b[42m");
	public static LogColor BgYellow { get; private set; } = new("\\x1b[43m");
	public static LogColor BgBlue { get; private set; } = new("\\x1b[44m");
	public static LogColor BgMagenta { get; private set; } = new("\\x1b[45m");
	public static LogColor BgCyan { get; private set; } = new("\\x1b[46m");
	public static LogColor BgWhite { get; private set; } = new("\\x1b[47m");
	public static LogColor BgGray { get; private set; } = new("\\x1b[100m");
	
	private string Value { get; } = value;

	public override string ToString() {
		return Value;
	}

}
