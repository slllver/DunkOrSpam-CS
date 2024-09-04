namespace DunkOrSpam_CS.utils;

public class LogColor(string value) {
	
	public static LogColor Reset { get; private set; } = new("\x1B[0m");
	public static LogColor Bright { get; private set; } = new("\x1B[1m");
	public static LogColor Dim { get; private set; } = new("\x1B[2m");
	public static LogColor Underscore { get; private set; } = new("\x1B[4m");
	public static LogColor Blink { get; private set; } = new("\x1B[5m");
	public static LogColor Reverse { get; private set; } = new("\x1B[7m");
	public static LogColor Hidden { get; private set; } = new("\x1B[8m");
	
	public static LogColor FgBlack { get; private set; } = new("\x1B[30m");
	public static LogColor FgRed { get; private set; } = new("\x1B[31m");
	public static LogColor FgGreen { get; private set; } = new("\x1B[32m");
	public static LogColor FgYellow { get; private set; } = new("\x1B[33m");
	public static LogColor FgBlue { get; private set; } = new("\x1B[34m");
	public static LogColor FgMagenta { get; private set; } = new("\x1B[35m");
	public static LogColor FgCyan { get; private set; } = new("\x1B[36m");
	public static LogColor FgWhite { get; private set; } = new("\x1B[37m");
	public static LogColor FgGray { get; private set; } = new("\x1B[90m");
	
	public static LogColor BgBlack { get; private set; } = new("\x1B[40m");
	public static LogColor BgRed { get; private set; } = new("\x1B[41m");
	public static LogColor BgGreen { get; private set; } = new("\x1B[42m");
	public static LogColor BgYellow { get; private set; } = new("\x1B[43m");
	public static LogColor BgBlue { get; private set; } = new("\x1B[44m");
	public static LogColor BgMagenta { get; private set; } = new("\x1B[45m");
	public static LogColor BgCyan { get; private set; } = new("\x1B[46m");
	public static LogColor BgWhite { get; private set; } = new("\x1B[47m");
	public static LogColor BgGray { get; private set; } = new("\x1B[100m");
	
	private string Value { get; } = value;

	public override string ToString() {
		return Value;
	}

}
