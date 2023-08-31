<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\System.Windows.Forms.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Security.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Configuration.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\Accessibility.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Deployment.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Runtime.Serialization.Formatters.Soap.dll</Reference>
  <NuGetReference>System.Reactive</NuGetReference>
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Drawing.Imaging</Namespace>
  <Namespace>System.Reactive</Namespace>
  <Namespace>System.Reactive.Linq</Namespace>
  <Namespace>System.Runtime.InteropServices</Namespace>
  <Namespace>System.Reactive.Disposables</Namespace>
  <Namespace>System.Windows.Forms</Namespace>
</Query>

// type: utility, image
// description: screen magnifier, press F6 to take snapshot
// setup: these constants below
const ModifierKeys Modifiers = ModifierKeys.None;
const Keys Hotkey = Keys.F6;
const int PollingRate = 125;

void Main()
{
	var colorAtCursor = Observable.Interval(TimeSpan.FromMilliseconds(PollingRate))
		.Select(_ => API.GetCursorPosition())
		.Select(position => new { Position = position.ToString(), Color = API.GetColorAt(position) })
		.Publish().RefCount();

	var trigger = Observable.Using(
		() => new KeyboardHook().RegisterHotKey(Modifiers, Hotkey),
		hook => Observable.FromEventPattern<KeyPressedEventArgs>(
			h => hook.KeyPressed += h,
			h => hook.KeyPressed -= h));
			
	Util.Metatext($" // Press {(Modifiers == ModifierKeys.None ? "" : Modifiers + "+")}{Hotkey} to take a snapshot").Dump();
	colorAtCursor
		.Select(x => new
		{
			Visual = x.Color.ToColoredBlock(),
			x.Position,
			x.Color,
		})
		.DumpLatest($"Color At Cursor");
	trigger.CombineLatest(colorAtCursor, Tuple.Create)
		.Distinct(x => x.Item1)
		.Select(x => x.Item2)
		.Select(x => Util.VerticalRun(
			x.Color.ToColoredBlock(),
			x.Position.ToString(),
			x.Color.ToPrettyString()
		))
		.Dump($"Snapshots");
}

// Define other methods and classes here
public static class API
{
	[DllImport("user32.dll")]
	private static extern bool GetCursorPos(ref Point lpPoint);
	public static Point GetCursorPosition()
	{
		Point p = Point.Empty;

		return GetCursorPos(ref p) ? p : Point.Empty;
	}
	
	[DllImport("gdi32.dll", CharSet = CharSet.Auto, SetLastError = true, ExactSpelling = true)]
	private static extern int BitBlt(IntPtr hDC, int x, int y, int nWidth, int nHeight, IntPtr hSrcDC, int xSrc, int ySrc, int dwRop);
	private static Bitmap screenPixel = new Bitmap(1, 1, PixelFormat.Format32bppArgb);
	public static Color GetColorAt(Point location)
	{
		using (Graphics gdest = Graphics.FromImage(screenPixel))
		{
			using (Graphics gsrc = Graphics.FromHwnd(IntPtr.Zero))
			{
				IntPtr hSrcDC = gsrc.GetHdc();
				IntPtr hDC = gdest.GetHdc();
				int retval = BitBlt(hDC, 0, 0, 1, 1, hSrcDC, location.X, location.Y, (int)CopyPixelOperation.SourceCopy);
				gdest.ReleaseHdc();
				gsrc.ReleaseHdc();
			}
		}

		return screenPixel.GetPixel(0, 0);
	}
}

#region KeyboardHook
// source: https://stackoverflow.com/a/27309185

public sealed class KeyboardHook : IDisposable
{
	// Registers a hot key with Windows.
	[DllImport("user32.dll")]
	private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
	// Unregisters the hot key with Windows.
	[DllImport("user32.dll")]
	private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

	/// <summary>
	/// Represents the window that is used internally to get the messages.
	/// </summary>
	private class Window : NativeWindow, IDisposable
	{
		private static int WM_HOTKEY = 0x0312;

		public Window()
		{
			// create the handle for the window.
			this.CreateHandle(new CreateParams());
		}

		/// <summary>
		/// Overridden to get the notifications.
		/// </summary>
		/// <param name="m"></param>
		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);

			// check if we got a hot key pressed.
			if (m.Msg == WM_HOTKEY)
			{
				// get the keys.
				Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
				ModifierKeys modifier = (ModifierKeys)((int)m.LParam & 0xFFFF);

				// invoke the event to notify the parent.
				if (KeyPressed != null)
					KeyPressed(this, new KeyPressedEventArgs(modifier, key));
			}
		}

		public event EventHandler<KeyPressedEventArgs> KeyPressed;

		#region IDisposable Members

		public void Dispose()
		{
			this.DestroyHandle();
		}

		#endregion
	}

	private Window _window = new Window();
	private int _currentId;

	public KeyboardHook()
	{
		// register the event of the inner native window.
		_window.KeyPressed += delegate (object sender, KeyPressedEventArgs args)
		{
			if (KeyPressed != null)
				KeyPressed(this, args);
		};
	}

	/// <summary>
	/// Registers a hot key in the system.
	/// </summary>
	/// <param name="modifier">The modifiers that are associated with the hot key.</param>
	/// <param name="key">The key itself that is associated with the hot key.</param>
	public KeyboardHook RegisterHotKey(ModifierKeys modifier, Keys key)
	{
		// increment the counter.
		_currentId = _currentId + 1;

		// register the hot key.
		if (!RegisterHotKey(_window.Handle, _currentId, (uint)modifier, (uint)key))
			throw new InvalidOperationException("Couldn’t register the hot key.");
		
		return this;
	}

	/// <summary>
	/// A hot key has been pressed.
	/// </summary>
	public event EventHandler<KeyPressedEventArgs> KeyPressed;

	#region IDisposable Members

	public void Dispose()
	{
		// unregister all the registered hot keys.
		for (int i = _currentId; i > 0; i--)
		{
			UnregisterHotKey(_window.Handle, i);
		}

		// dispose the inner native window.
		_window.Dispose();
	}

	#endregion
}

/// <summary>
/// Event Args for the event that is fired after the hot key has been pressed.
/// </summary>
public class KeyPressedEventArgs : EventArgs
{
	private ModifierKeys _modifier;
	private Keys _key;

	internal KeyPressedEventArgs(ModifierKeys modifier, Keys key)
	{
		_modifier = modifier;
		_key = key;
	}

	public ModifierKeys Modifier
	{
		get { return _modifier; }
	}

	public Keys Key
	{
		get { return _key; }
	}
}

/// <summary>
/// The enumeration of possible modifiers.
/// </summary>
[Flags]
public enum ModifierKeys : uint
{
	None = 0,
	Alt = 1,
	Control = 2,
	Shift = 4,
	Win = 8
}

#endregion

public static class ColorHelper
{
	public static string ToRgbText(this Color color) => $"{color.R:X2}{color.G:X2}{color.B:X2}";

	public static object ToColoredBlock(this Color color)
	{
		var background = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
		return Util.RawHtml($"<div style='height: 20px; width: 20px; background: #{color.ToRgbText()};' />");
	}

	public static string ToPrettyString(this Color color)
	{
		return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2} (rgb: {color.R} {color.G} {color.B}, alpha: {color.A})";
	}
}