<Query Kind="Program">
  <Namespace>System.Drawing</Namespace>
  <Namespace>System.Drawing.Drawing2D</Namespace>
  <Namespace>System.Drawing.Imaging</Namespace>
</Query>

void Main()
{
	Util.AutoScrollResults = false;
	
	// note 20230306: doesnt work anymore, just use the screenshot button of droid emulator, and look in the pc download folder
	// note: "A generic error occurred in GDI+." could just mean the folder doesnt exist... ffs
	//Screenshot().Save(GetCacheScreenshotPath().Dump(), ImageFormat.Jpeg); return;

	var screenshot = 
		//Screenshot();
		//Image.FromFile(GetCacheScreenshotPath());
		//Image.FromFile(@"C:\Users\Xiaoy312\Pictures\Xamarin\iOS Simulator\2021-10-06_12-56-43-PM.png"); // native
		//Image.FromFile(@"C:\Users\Xiaoy312\Downloads\asd.bmp"); // custom
		//Image.FromFile(@"C:\Users\Xiaoy312\Pictures\Xamarin\iOS Simulator\2021-10-06_03-52-47-PM.png"); // custom + workaround
		//Image.FromFile(@"C:\Users\Xiaoy312\Downloads\flyout skia.png");
		Image.FromFile(@"C:\Users\Xiaoy312\Downloads\Screenshot_4V.png");
		//Image.FromFile(@"D:\code\uno\framework\uno\src\SamplesApp\SamplesApp.UITests\bin\Debug\When_Disabled_After.png");
		//new Bitmap(500, 300) as Image;
	var profile = 
		//DeviceProfiles.Honor10.SetDisplayScaling(1.0 / 6.0);
		//DeviceProfiles.iPhone13ProMax.SetDisplayScaling(1.0 / 2.0);
		//DeviceProfiles.iPhone14ProMax.SetL2PScaling(3).SetDisplayScaling(1 / 5.0); // L2P can be 3 or 4
		DeviceProfiles.Android10Q.SetDisplayScaling(1.0 / 4);
		//new DeviceProfile(screenshot.Size, 1.5).SetDisplayScaling(1.0 / 3.0); // ios
		//new DeviceProfile(screenshot.Size, 1).SetDisplayScaling(1.0 / 3.0); // macos: variable window size
		//new DeviceProfile(screenshot.Size, 2.625/* Uno.UI.ViewHelper.Scale */).SetDisplayScaling(1.0 / 4.0); // droid emulator
		//new DeviceProfile(screenshot.Size, 1).SetDisplayScaling(1); // generic uwp/skia?
		//new DeviceProfile(screenshot.Size, 1).SetDisplayScaling(1.0/4.0); // no scaling

	// block some client stuffs
	//screenshot = screenshot
	//	.Overlay(new Rectangle(0, 30, 360, 200).Scale(profile.LogicalToPhysicalScaling), Color.White)
	//	.Overlay(new Rectangle(0, 720, 180, 40).Scale(profile.LogicalToPhysicalScaling), Color.White);

	Func<int, int> scalePaint = x => x / 2;
	Func<Image, Point, Image> crosshair = (image, x) => image.Crosshair(x, scalePaint(200), Color.Red, scalePaint(5));
	Func<Image, Point, Image> crosshairBlack = (image, x) => image.Crosshair(x, scalePaint(200), Color.Black, scalePaint(5));
	Func<Image, Point, Image> crosshairThin = (image, x) => image.Crosshair(x, scalePaint(10), Color.Red, scalePaint(1));
	Func<Image, (Point, Point), Image> lineThin = (image, x) => image.Line(x.Item1, x.Item2, Color.Red, scalePaint(1));
	Func<Image, Rectangle, Image> overlay = (image, x) => image.Overlay(x, Color.Black, 0.5);
	Func<Image, Rectangle, Image> overlayPink = (image, x) => image.Overlay(x, Color.Pink, 0.7);
	Func<Image, Rectangle, Image> overlayPinkWithCrosshair = (image, x) => image.Overlay(x, Color.Black, 0.7).Crosshair(x.GetCenter(), scalePaint(50), Color.Red, scalePaint(5));
	Func<Image, Rectangle, Image> overlayCrosshairEdge = (image, x) => image.Overlay(x, Color.Pink, 0.7).Crosshair(x.GetCenter(), scalePaint(50), Color.Red, scalePaint(5)).Edge(x, Color.Red, scalePaint(5));
	Func<Image, Rectangle, Image> overlayBigCrosshairEdge = (image, x) => image
		.Overlay(x, Color.Pink, 0.7)
		.Crosshair(x.GetCenter(), 1000, Color.Red, scalePaint(2))
		.Edge(x, Color.Red, scalePaint(5));

	//screenshot = screenshot.Crop(120, screenshot.Height);
	Util.HorizontalRun(true,
		StackView("screencap", screenshot, (ss, x) => ss),
		//StackView("windowRect", new Rectangle(0, 0, 360, 760).Scale(profile.LogicalToPhysicalScaling), overlayPink),
		//StackView("visibleBounds", new Rectangle(0, 30, 360, 730).Scale(profile.LogicalToPhysicalScaling), x => HighlightArea(screenshot.Size, x)),
		" ", // separator
		
		//StackView("MainPage", FromDiagnostic("[1080x1731@0,0]").Translate(dy: 63), overlayPink), 
		StackView("containing Grid", FromDiagnostic("[1080x1490@0,126]").Translate(dy: 63), overlayCrosshairEdge), // 63 offset for status-bar
		StackView("StackPanel", FromDiagnostic("[1080x348@0,0]").Translate(dy: 63+126), overlayCrosshairEdge), // 126 inherited offset from its parent
		StackView("ListView", FromDiagnostic("[1080x1258@0,348]").Translate(dy: 63+126), overlayCrosshairEdge),
		StackView("Buttons",
			new[] { "[201x115@0,1470]", "[201x115@440,1470]", "[201x115@879,1470]" }.Select(x => FromDiagnostic(x).Translate(dy: 63+126)),
			(ss, ctx) => ctx.Aggregate(ss, overlayCrosshairEdge),
			rects => string.Join("\n", rects)
		),
		
		//StackView("Window\\UIView[1]\\UIView[1]", FromDiagnostic("[Rect 430x863@0,69]").ScaleFor(profile), overlayCrosshairEdge),
		//StackView("ComboBox[1]", FromDiagnostic("[Rect 398x48@16,117]").ScaleFor(profile), overlayCrosshairEdge),
		//StackView("Popup[1]\\Border#PopupBorder", FromDiagnostic("[Rect 398x48@0,59]").ScaleFor(profile), overlayCrosshairEdge),
		
		//StackView("LVI#11\\Text actual", FromDiagnostic("[Rect 364x191@16,52]").ScaleFor(profile), overlayCrosshairEdge),
		//StackView("LVI#11\\Text BBox%0", FromDiagnostic("[Rect 364x115@16,52]").ScaleFor(profile), overlayCrosshairEdge),
		//StackView("LVI#11\\Text BBox%1", FromDiagnostic("[Rect 364x153@16,52]").ScaleFor(profile), overlayCrosshairEdge),
		//StackView("LVI#11\\Text BBox%2", FromDiagnostic("[Rect 364x191@16,52]").ScaleFor(profile), overlayCrosshairEdge),
		
		//StackView("viewport", ParseRect("[365.7x459.8@23,201]").ScaleFor(profile), overlayCrosshairEdge),
		//StackView("viewport-kb", ParseRect("[365.7x200@23,201]").ScaleFor(profile), overlayCrosshairEdge),
		//StackView("asd", ParseRect("[Rect {330.666666666667,54.4761904761905}@{[35.0476188659668, 321.904761904762]}]").Translate(tSV).ScaleFor(profile), overlayCrosshairEdge),
		//StackView("13 AutoLayout", FromDiagnostic("[Rect 428x84@0,30]").Translate(0, 5).ScaleFor(profile), overlayCrosshairEdge),
		//StackView("15 PART_RootGrid", FromDiagnostic("[Rect 392x48@18,18]").Translate(0, 5+30).ScaleFor(profile), overlayCrosshairEdge),
		//StackView("24 Grid", FromDiagnostic("[Rect 309x24@0,0]").Translate(18, 5+30+18).ScaleFor(profile), overlayCrosshairEdge),
		//StackView("25 PathIcon", FromDiagnostic("[Rect 21x21.333333333333336@0,1.333333333333334]").Translate(18, 5+30+18).ScaleFor(profile), overlayCrosshairEdge),
		//StackView("27 TextBlock", FromDiagnostic("[Rect 282x24@27,0]").Translate(18, 5+30+18).ScaleFor(profile), overlayCrosshairEdge),
		
		//StackView("27 TextBlock", FromDiagnostic("[Rect 282x24@27,0]").Translate(18, 5+30+18).ScaleFor(profile), overlayCrosshairEdge),
		
		//StackView("FeedView ", new Point(8, 50), crosshairBlack),
		
		/* ios syntax */
		//StackView("785: FlyoutBasePopupPanel", ParseRect("(428x926)@(0,0)").Scale(profile.LogicalToPhysicalScaling), overlayCrosshairEdge),
		//StackView("786: VisibleBounds", ParseRect("(428x845)@(0,47)").Scale(profile.LogicalToPhysicalScaling), overlayCrosshairEdge),
		
		
		//StackView("finalFrame4 &", 
		//	new[] { "[Rect 400x60.1904761904762@365.714294433594,40.3809509277344]", "[Rect 400x60.1904761904762@315.714294433594,40.3809509277344]" },
		//	ctx => ctx.Aggregate(screenshot, (canvas, x) => canvas.Edge(FromDiagnostic(x).Translate(offset).Scale(profile.LogicalToPhysicalScaling), Color.Red))
		//),
		//StackView("25PathIcon + 27TextBlock",
		//	new[] { "[Rect 309x24@0,0]", "[Rect 21x21.333333333333336@0,1.333333333333334]", "[Rect 282x24@27,0]" }.Select(x => FromDiagnostic(x).Translate(18, 5+30+18).ScaleFor(profile)),
		//	(ss, ctx) => ctx.Skip(1).Aggregate(ss, overlayBigCrosshairEdge)
		//),

		""
	).Dump();

	object StackView<T>(string header, T value, Func<Image, T, Image> visualize, Func<T, string> describe = null)
	{
		describe = describe ?? (x => GetCustomDescription(x) ?? x.ToString());

		return Util.VerticalRun(
			header,
			visualize(screenshot, value).Scale(profile.DisplayScaling),
			describe(value)
		);
	}
	object StackViewEx(string header, Func<Image, Image> visualize)
	{
		return Util.VerticalRun(
			header,
			visualize(screenshot).Scale(profile.DisplayScaling),
			null
		);
	}
	string GetCustomDescription(object o)
	{
		switch (o)
		{
			case Image x: return GetPhysicalAndLogicalDescription(x.Size, y => y.Scale(profile.PhysicalToLogicalScaling), y => $"Size: w={y.Width}, h={y.Height}");
			case Rectangle x: return GetPhysicalAndLogicalDescription(x, y => y.Scale(profile.PhysicalToLogicalScaling), y => $"LTWH={y.X},{y.Y},{y.Width}x{y.Height}");
			case Point x: return GetPhysicalAndLogicalDescription(x, y => y.Scale(profile.PhysicalToLogicalScaling), y => $"Point: x={y.X}, y={y.Y}");

			case null: return null;
			default: return null;
		}

		string GetPhysicalAndLogicalDescription<T>(T value, Func<T, T> logicalConverter, Func<T, string> describe) => profile.LogicalToPhysicalScaling != 1
			? string.Concat("[L]", describe(logicalConverter(value)), "\n", "[P]", describe(value))
			: describe(value);
	}
}

string GetCacheScreenshotPath()
{
	return Path.Combine(
		Environment.GetFolderPath(Environment.SpecialFolder.Personal), 
		"Downloads",
		"phone-capture.jpg"
	);
}
System.Drawing.Image Screenshot()
{
	var process = new Process();
	process.StartInfo.FileName = "adb";
	process.StartInfo.Arguments = "exec-out screencap -p";
	process.StartInfo.UseShellExecute = false;
	process.StartInfo.RedirectStandardOutput = true;
	process.StartInfo.CreateNoWindow = true;

	process.Start();

	using (var stream = new MemoryStream())
	{
		process.StandardOutput.BaseStream.CopyTo(stream);
		process.WaitForExit();
		
		return Image.FromStream(stream);
	}
}

Rectangle FromLTRB(int left, int top, int right, int bottom) => new Rectangle(left, top, right - left, bottom - top);
Rectangle FromDiagnostic(string text)
{
	var match = Regex.Match(text, @"^\[(Rect )?(?<width>\d+(\.\d+)?)(x|,)(?<height>\d+(\.\d+)?)@(?<x>\d+(\.\d+)?), ?(?<y>\d+(\.\d+)?)\]$");
	if (!match.Success)
		throw new FormatException("Unable to parse: " + text);
	
	return new Rectangle
	(
		(int)double.Parse(match.Groups["x"].Value),
		(int)double.Parse(match.Groups["y"].Value),
		(int)double.Parse(match.Groups["width"].Value),
		(int)double.Parse(match.Groups["height"].Value)
	);
}
Rectangle ParseRect(string text)
{
	var patterns = new (string Pattern, Func<Match, Rectangle> Parser)[]
	{
		( $@"^\({Capture<decimal>("x")} {Capture<decimal>("y")}; {Capture<decimal>("width")} {Capture<decimal>("height")}\)", RectangleFromXYWH ), 		// (0 0; 414 896)
		( $@"^\({Capture<decimal>("width")}x{Capture<decimal>("height")}\)@\({Capture<decimal>("x")},{Capture<decimal>("y")}\)", RectangleFromXYWH ),	// (414x896)@(0,0) 
		( $@"^\[(Rect )?{Capture<decimal>("width")}x{Capture<decimal>("height")}@{Capture<decimal>("x")},{Capture<decimal>("y")}\]", RectangleFromXYWH ), 	// [Rect 414x896@0,0]
		( $@"^\[Rect {{{Capture<decimal>("width")},{Capture<decimal>("height")}}}@{{\[{Capture<decimal>("x")}, ?{Capture<decimal>("y")}\]}}\]", RectangleFromXYWH ),	// [Rect {330,54}@{[35, 197]}]  // uno4.x ToString format...
	};
	foreach (var item in patterns)
	{
		if (Regex.Match(text, item.Pattern) is var m && m.Success)
			return item.Parser(m);
	}
	
	throw new FormatException("Unable to parse: " + text);
	
	string Capture<T>(string name) => Type.GetTypeCode(typeof(T)) switch
	{
		TypeCode.Int32 => $"(?<{name}>{@"-?\d+"})",
		TypeCode.Decimal => $"(?<{name}>{@"-?\d+(\.\d+)?"})",
		_ => throw new ArgumentOutOfRangeException(),
	};
	Rectangle RectangleFromXYWH(Match match) => new Rectangle
	(
		(int)double.Parse(match.Groups["x"].Value),
		(int)double.Parse(match.Groups["y"].Value),
		(int)double.Parse(match.Groups["width"].Value),
		(int)double.Parse(match.Groups["height"].Value)
	);
}
Image HighlightArea(Size screen, Rectangle rectangle)
{
	var image = new Bitmap(screen.Width, screen.Height);
	using (var g = Graphics.FromImage(image))
	{
		g.Clear(Color.Pink);
		g.FillRectangle(Brushes.Orange, rectangle);
	}

	return image;
}

public record DeviceProfile(
	Size PhysicalScreenSize, 
	double LogicalToPhysicalScaling, // ViewHelper.Scale
	double DisplayScaling = 1.0 / 6.0) // 
{
	public Size LogicalScreenSize => PhysicalScreenSize.Scale(PhysicalToLogicalScaling);

	// You should use the value of `` for log2phy
	public double PhysicalToLogicalScaling => Math.Pow(LogicalToPhysicalScaling, -1);
	
	public DeviceProfile(int physicalScreenWidth, int physicalScreenHeight, double logicalToPhysicalScaling, double displayScaling = 1.0 / 6.0)
		: this(new Size(physicalScreenWidth, physicalScreenHeight), logicalToPhysicalScaling, displayScaling)
	{
	}

	// fluent syntax
	public DeviceProfile SetDisplayScaling(double value)
	{
		return this with { DisplayScaling = value };
	}
	public DeviceProfile SetL2PScaling(double value)
	{
		return this with { LogicalToPhysicalScaling = value };
	}
}
public class DeviceProfiles
{
	public static readonly DeviceProfile Honor10 = new DeviceProfile(360 * 3, 760 * 3, 3, 1.0 / 6);
	public static readonly DeviceProfile iPhone12ProMax = new DeviceProfile(642, 1389, 1.5, 1.0 / 3);
	public static readonly DeviceProfile iPhone13ProMax = new DeviceProfile(1284, 2778, 3, 1.0 / 3);
	public static readonly DeviceProfile iPhone14ProMax = new DeviceProfile(1290, 2796, 3, 1.0 / 4);
	public static readonly DeviceProfile Android10Q = new DeviceProfile(1080, 1920, 2.625, 1.0 / 3);
}

public static class ImageExtensions
{
    public static Image Crop(this Image image, int width, int height) => image.Crop(0, 0, width, height);
    public static Image Crop(this Image image, int x, int y, int width, int height) => image.Crop(new Rectangle(x, y, width, height));
	public static Image Crop(this Image image, Rectangle rect)
	{
		return (image as Bitmap ?? new Bitmap(image)).Clone(rect, image.PixelFormat);
	}
	
	public static Bitmap Scale(this System.Drawing.Image image, double factor) => image.Scale((int)(image.Width * factor), (int)(image.Height * factor));
	public static Bitmap Scale(this System.Drawing.Image image, int width, int height)
	{
		var destRect = new Rectangle(0, 0, width, height);
		var destImage = new Bitmap(width, height);

		destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

		using (var graphics = Graphics.FromImage(destImage))
		{
			graphics.CompositingMode = CompositingMode.SourceCopy;
			graphics.CompositingQuality = CompositingQuality.HighQuality;
			graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
			graphics.SmoothingMode = SmoothingMode.HighQuality;
			graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

			using (var wrapMode = new ImageAttributes())
			{
				wrapMode.SetWrapMode(WrapMode.TileFlipXY);
				graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
			}
		}

		return destImage;
	}

	public static Image Overlay(this Image source, Rectangle rectangle, Color color, double opacity = 1.0) => source.Overlay(rectangle, new SolidBrush(Color.FromArgb((int)(255 * opacity), color.R, color.G, color.B)));
	public static Image Overlay(this Image source, Rectangle rectangle, Brush brush)
	{
		var image = new Bitmap(source);
		using (var g = Graphics.FromImage(image))
		{
			g.FillRectangle(brush, rectangle);
		}

		return image;
	}

	public static Image Edge(this Image source, Rectangle rectangle, Color color, int thickness = 5) => source.Edge(rectangle, new Pen(color, thickness){ Alignment = PenAlignment.Inset });
	public static Image Edge(this Image source, Rectangle rectangle, Pen pen)
	{
		var image = new Bitmap(source);
		using (var g = Graphics.FromImage(image))
		{
			g.DrawLines(pen, new[]
			{
				new Point(rectangle.Left, rectangle.Top),
				new Point(rectangle.Left, rectangle.Bottom),
				new Point(rectangle.Right, rectangle.Bottom),
				new Point(rectangle.Right, rectangle.Top),
				new Point(rectangle.Left, rectangle.Top),
			});
		}

		return image;
	}

	public static Image Line(this Image source, Point p0, Point p1, Color color, int thickness = 5) => source.Line(p0, p1, new Pen(color, thickness){ Alignment = PenAlignment.Inset });
	public static Image Line(this Image source, Point p0, Point p1, Pen pen)
	{
		var image = new Bitmap(source);
		using (var g = Graphics.FromImage(image))
		{
			g.DrawLines(pen, new[] { p0, p1 });
		}

		return image;
	}
	
	public static Image Crosshair(this Image source, Point location, int length, Color color, int thickness = 5) => source.Crosshair(location, length, new Pen(color, thickness){ Alignment = PenAlignment.Center });
	public static Image Crosshair(this Image source, Point location, int length, Pen pen)
	{
		var image = new Bitmap(source);
		using (var g = Graphics.FromImage(image))
		{
			g.DrawLine(pen, location.X - length, location.Y, location.X + length, location.Y);
			g.DrawLine(pen, location.X, location.Y - length, location.X, location.Y + length);
		}

		return image;
	}
	
	public static Image Rect(this Image source, Rectangle rect, Color color, int thickness = 5) => source.Rect(rect, new Pen(color, thickness){ Alignment = PenAlignment.Center });
	public static Image Rect(this Image source, Rectangle rect, Pen pen)
	{
		var image = new Bitmap(source);
		using (var g = Graphics.FromImage(image))
		{
			g.DrawRectangle(pen, rect);
		}

		return image;
	}
	
	public static Image Draw(this Image source, Action<Graphics> draw)
	{
		var image = new Bitmap(source);
		using (var g = Graphics.FromImage(image))
		{
			draw(g);
		}

		return image;
	}
}
public static class SyntaxExtensions
{
	public static T Apply<T>(this T target, Action<T> action) { action(target); return target; }
	public static T ApplyIf<T>(this T target, bool condition, Action<T> action) { if (condition) action(target); return target; }
	
	public static TResult Apply<T, TResult>(this T target, Func<T, TResult> selector) => selector(target);
	public static TResult Apply<T, TArgument, TResult>(this T target, Func<T, TArgument, TResult> selector, TArgument argument) => selector(target, argument);
	
}
public static class GeometryExtensions
{
	public static Point Scale(this Point point, double factor)
	{
		return new Point((int)(point.X * factor), (int)(point.Y * factor));
	}
	public static Size Scale(this Size size, double factor)
	{
		return new Size((int)(size.Width * factor), (int)(size.Height * factor));
	}
	public static Rectangle Scale(this Rectangle rect, double factor)
	{
		return new Rectangle((int)(rect.X * factor), (int)(rect.Y * factor), (int)(rect.Width * factor), (int)(rect.Height * factor));
	}
	
	public static Point Translate(this Point p, Point delta) => p.Translate(delta.X, delta.Y);
	public static Point Translate(this Point p, int dx, int dy) => new Point(p.X + dx, p.Y + dy);
	public static Rectangle Translate(this Rectangle rect, Point delta) => rect.Translate(delta.X, delta.Y);
	public static Rectangle Translate(this Rectangle rect, int dx = 0, int dy = 0) => new Rectangle(rect.X + dx, rect.Y + dy, rect.Width, rect.Height);
	public static Rectangle Translate(this Rectangle rect, double dx = 0, double dy = 0) => new Rectangle((int)(rect.X + dx), (int)(rect.Y + dy), rect.Width, rect.Height);
	
	public static Point GetCenter(this Rectangle rect) => rect.Location.Translate(rect.Size.Scale(0.5).AsPoint());
	
	public static Point AsPoint(this Size size) => new Point(size.Width, size.Height);
}
internal static class CustomGeometryExtensions
{
	public static Rectangle ScaleFor(this Rectangle rect, DeviceProfile profile) => rect.Scale(profile.LogicalToPhysicalScaling);
	public static Point ScaleFor(this Point p, DeviceProfile profile) => p.Scale(profile.LogicalToPhysicalScaling);
}