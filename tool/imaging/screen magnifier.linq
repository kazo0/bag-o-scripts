<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\Accessibility.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Configuration.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Deployment.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Runtime.Serialization.Formatters.Soap.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Security.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Windows.Forms.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Xaml.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\WPF\WindowsBase.dll</Reference>
  <NuGetReference>System.Reactive</NuGetReference>
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Drawing.Drawing2D</Namespace>
  <Namespace>System.Drawing.Imaging</Namespace>
  <Namespace>System.Reactive.Linq</Namespace>
  <Namespace>System.Runtime.InteropServices</Namespace>
  <Namespace>System.Windows.Forms</Namespace>
  <Namespace>System.Windows.Input</Namespace>
</Query>

// type: utility, image
// description: screen magnifier, press F6 to take snapshot
// setup: these constants below
const ModifierKeys Modifiers = ModifierKeys.None;
const Keys Hotkey = Keys.F6;
const int PollingRate = 500;
const int ScaleFactor = 5;
readonly Size CaptureSize = new Size(100, 100);

void Main()
{
	var imageAtCursor = Observable.Interval(TimeSpan.FromMilliseconds(PollingRate))
		.Select(_ => API.GetCursorPosition())
		.Select(position => new
		{
			Position = position.ToString(),
			Image = API.Screenshot(position.CenterOffset(CaptureSize), CaptureSize).ScaleUp(ScaleFactor),
		})
		.Publish().RefCount();
	Util.Metatext($" // Press {(Modifiers == ModifierKeys.None ? "" : Modifiers + "+")}{Hotkey} to take a snapshot").Dump();
	
	var container = new DumpContainer().Dump();
	imageAtCursor.Subscribe(x => container.Content = Util.VerticalRun(
		x.Position,
		x.Image
	));
	//imageAtCursor.DumpLatest();

	var trigger = Observable.Using(
		() => new KeyboardHook().RegisterHotKey(Modifiers, Hotkey),
		hook => Observable.FromEventPattern<KeyPressedEventArgs>(
			h => hook.KeyPressed += h,
			h => hook.KeyPressed -= h));
	trigger.CombineLatest(imageAtCursor, Tuple.Create)
		.Distinct(x => x.Item1)
		.Select(x => x.Item2)
		.Select(x => Util.VerticalRun(
			x.Position.ToString(),
			x.Image
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

	public static Bitmap Screenshot(int x, int y, int width, int height) => Screenshot(new Point(x, y), new Size(width, height));
	public static Bitmap Screenshot(Point location, Size size)
	{
		var result = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb);

		using (var source = Graphics.FromHwnd(IntPtr.Zero))
		using (var destination = Graphics.FromImage(result))
		{
			var ret = BitBlt(
				destination.GetHdc(),
				0, 0,
				size.Width, size.Height,
				source.GetHdc(),
				location.X, location.Y,
				(int)CopyPixelOperation.SourceCopy);

			source.ReleaseHdc();
			destination.ReleaseHdc();
		}

		return result;
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
public static class PointExtensions
{
	public static Point CenterOffset(this Point p, Size region)
	{
		return new Point(p.X - region.Width / 2, p.Y - region.Height / 2);
	}
}
public static class ImageExtensions
{
	public static Bitmap ScaleUp(this Bitmap image, int scale)
	{
		if (scale < 1) throw new ArgumentException();
		if (scale == 1) return image;

		var result = new Bitmap(image.Width * scale, image.Height * scale, image.PixelFormat);
		for (int x = 0; x < image.Width; x++)
			for (int y = 0; y < image.Height; y++)
			{
				var color = image.GetPixel(x, y);

				for (int dx = scale * x; dx < scale *x + scale; dx++)
					for (int dy = scale * y; dy < scale * y + scale; dy++)
						result.SetPixel(dx, dy, color);
			}

		return result;
	}
}