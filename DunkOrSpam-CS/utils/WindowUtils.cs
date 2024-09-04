using System.Runtime.InteropServices;

namespace DunkOrSpam_CS.utils;

public static class WindowUtils {

	#region Scroll info constants

	private const int SbHorz = 0;
	private const int SbVert = 1;
	private const int SbCtl = 2;

	#endregion

	#region Flash window constants

	//Stop flashing. The system restores the window to its original state.
	const UInt32 FlashwStop = 0;

	//Flash the window caption.
	const UInt32 FlashwCaption = 1;

	//Flash the taskbar button.
	const UInt32 FlashwTray = 2;

	//Flash both the window caption and taskbar button.
	//This is equivalent to setting the FLASHW_CAPTION | FLASHW_TRAY flags.
	const UInt32 FlashwAll = 3;

	//Flash continuously, until the FLASHW_STOP flag is set.
	const UInt32 FlashwTimer = 4;

	//Flash continuously until the window comes to the foreground.
	const UInt32 FlashwTimernofg = 12;

	#endregion

	#region Extern function declarations

	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool FlashWindowEx(ref Flashwinfo pwfi);
	
	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.SysInt)]
	private static extern IntPtr FindWindowA(string lpClassName, string lpWindowName);
	
	[DllImport("user32.dll")]
	[return: MarshalAs(UnmanagedType.Bool)]
	private static extern bool GetScrollInfo(IntPtr hwnd, int nBar, ref LPSCROLLINFO lpsi);

	#endregion
	
	/// <summary>
	/// Gets the handle of the given window by name
	/// </summary>
	/// <param name="lpWindowName">The window name to find</param>
	/// <returns>IntPtr representing window handle</returns>
	public static IntPtr FindWindow(string lpWindowName) {
		return FindWindowA(null!, lpWindowName);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="handle"></param>
	/// <param name="timeout"></param>
	/// <param name="count"></param>
	/// <returns></returns>
	public static bool FlashWindow(IntPtr handle, UInt32 timeout, UInt32 count) {
		IntPtr hWnd = handle;
		Flashwinfo fInfo = new Flashwinfo();

		fInfo.cbSize = Convert.ToUInt32(Marshal.SizeOf(fInfo));
		fInfo.hwnd = hWnd;
		fInfo.dwFlags = FlashwAll | FlashwTimernofg;
		fInfo.uCount = count;
		fInfo.dwTimeout = timeout;

		return FlashWindowEx(ref fInfo);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <param name="hwnd"></param>
	/// <returns></returns>
	private static int GetScroll(IntPtr hwnd) {
		LPSCROLLINFO lpsi = new LPSCROLLINFO();

		lpsi.cbSize = Convert.ToUInt32(Marshal.SizeOf(lpsi));
		lpsi.fMask = (uint)ScrollInfoMask.SIF_ALL;
		// lpsi.fMask = 0;

		bool success = GetScrollInfo(hwnd, SbVert, ref lpsi);

		return success ? lpsi.nPos : -1;
	}

	public static async Task<int> InvokeScroll(IntPtr hwnd) {
		int scroll = -1;

		await Task.Run(() => scroll = GetScroll(hwnd));
		
		return scroll;
	}
	
	[StructLayout(LayoutKind.Sequential)]
	private struct Flashwinfo {
		public UInt32 cbSize;
		public IntPtr hwnd;
		public UInt32 dwFlags;
		public UInt32 uCount;
		public UInt32 dwTimeout;
	}
	
	private struct LPSCROLLINFO {
		public uint cbSize;
		public uint fMask;
		public int nMin;
		public int nMax;
		public uint nPage;
		public int nPos;
		public int nTrackPos;
	}
	
	private enum ScrollInfoMask : uint {
		SIF_RANGE = 0x1,
		SIF_PAGE = 0x2,
		SIF_POS = 0x4,
		SIF_DISABLENOSCROLL = 0x8,
		SIF_TRACKPOS = 0x10,
		SIF_ALL = (SIF_RANGE | SIF_PAGE | SIF_POS | SIF_TRACKPOS),
	}
	
}
