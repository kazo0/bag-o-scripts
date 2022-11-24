<Query Kind="Program">
  <Namespace>System.Windows</Namespace>
</Query>

using static UserQuery.Global;

void Main() => 
	LocateLocalizableElements(@"D:\code\uno\platform\Uno.Todo\src\ToDo.UI\Views\TaskControl.xaml", @"D:\code\uno\platform\Uno.Todo\src\ToDo.UI\Strings\fr\Resources.resw");

public static class Global
{
	public static readonly XNamespace Presentation = XNamespace.Get(NSPresentation);
	public static readonly XNamespace X = XNamespace.Get(NSX);

	public const string NSX = "http://schemas.microsoft.com/winfx/2006/xaml";
	public const string NSPresentation = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
}

void LocateLocalizableElements(string pathToXaml, string pathToResw)
{
	var filename = Path.GetFileNameWithoutExtension(pathToXaml);
	var xaml = XElement.Parse(File.ReadAllText(pathToXaml), LoadOptions.SetLineInfo);
	var resw = pathToResw == null ? default : XElement.Parse(File.ReadAllText(pathToResw))
			.Descendants("data")
			.ToDictionary(x => x.Attribute("name").Value, x => x.Value.Trim())
			.Dump("resw", 0);
	
	var elements = xaml.Descendants()
		.Where(x => !x.Name.LocalName.Contains(".")) // skip property node
		.Select(LocalizableElement.Parse)
		.ToArray()
		.Dump("elements", 0);
	
	elements
		.SelectMany(x => (x?.LocalizableProperties ?? Enumerable.Empty<LocalizableProperty>())
			.Select(y => new { y.Name, y.Value, FullPath = $"{x.Xuid}.{y.Name}", RelativePath = Regex.Replace($"{x.Xuid}.{y.Name}", $"{filename}_", "") })
			.Select(y => new
			{
				x.Name,
				x.Xuid,
				x.Line,
				Property = y.Name,
				RelativePath = Clickable.CopyText(y.RelativePath),
				//FullPath = Clickable.CopyText(y.FullPath),
				//IsLocalized = Util.HighlightIf(resw.ContainsKey(y.FullPath), x => !x),
				y.Value,
				Localized = resw.TryGetValue(y.FullPath, out var localized) ? localized : default
			}))
		.Dump($"localizable in {Path.GetFileName(pathToXaml)}", 1);
}

public partial class LocalizableElement
{
	public string Name { get; set; }
	public string Xuid { get; set; }
	public int Line { get; set; }
	public LocalizableProperty[] LocalizableProperties { get; set; }
}
public partial class LocalizableElement
{
	public static LocalizableElement Parse(XElement e)
	{
		return new LocalizableElement
		{
			Name = e.GetNiceName(),
			Xuid = e.Attribute(X + "Uid")?.Value,
			Line = (e as IXmlLineInfo).LineNumber,
			LocalizableProperties = GetLocalizableProperties(e)?.ToArray()
		};
	}
	public static IEnumerable<LocalizableProperty> GetLocalizableProperties(XElement e)
	{
		const string utu = "using:Uno.Toolkit.UI";
		const string uer = "using:Uno.Extensions.Reactive.UI";
		const string converters = "using:ToDo.Converters";
	
		return (e.Name.NamespaceName, e.Name.LocalName) switch
		{
			// control
			(NSPresentation, "AppBarButton") => GetDefinedAttributes("Content"),
			(NSPresentation, "Border") => GetDefinedAttributes("Content"),
			(NSPresentation, "Button") => GetDefinedAttributes("Content"),
			(NSPresentation, "BitmapIcon") => null,
			(NSPresentation, "CheckBox") => GetDefinedAttributes("Content"),
			(NSPresentation, "ContentControl") => GetDefinedAttributes("Content"),
			(NSPresentation, "Grid") => null,
			(NSPresentation, "ListView") => null,
			(NSPresentation, "PathIcon") => null,
			(NSPresentation, "PathIconSource") => null,
			(NSPresentation, "PersonPicture") => GetDefinedAttributes("DisplayName", "Initials"),
			(NSPresentation, "SwipeControl") => GetDefinedAttributes("Content"),
			(NSPresentation, "SwipeItems") => GetDefinedAttributes("Content"),
			(NSPresentation, "SwipeItem") => GetDefinedAttributes("Content"),
			(NSPresentation, "TextBlock") => GetDefinedAttributes("Text"),
			(NSPresentation, "TextBox") => GetDefinedAttributes("Text", "PlaceholderText"),
			(NSPresentation, "ToggleButton") => GetDefinedAttributes("Content"),
			(utu, "AutoLayout") => null,
			(utu, "Chip") => GetDefinedAttributes("Content"),
			(utu, "ChipGroup") => null,
			(utu, "Divider") => null,
			(utu, "NavigationBar") => GetDefinedAttributes("Content"),
			(utu, "TabBar") => null,
			(utu, "TabBarItem") => GetDefinedAttributes("Content"),
			(uer, "FeedView") => GetDefinedAttributes("Content"),
			
			// converters
			(converters, _) => SelectAttributes(x => x.Name.Namespace != NSX),
			
			// misc
			(NSX, "String") => null,
			(NSPresentation, "StaticResource") => null,
			(NSPresentation, "ItemsPanelTemplate") => null,
			(NSPresentation, "ItemsStackPanel") => null,
			(NSPresentation, "DataTemplate") => null,
			(NSPresentation, "Style") => null,
			(NSPresentation, "Setter") => null,
			(NSPresentation, "VisualStateGroup") => null,
			(NSPresentation, "VisualState") => null,
			(NSPresentation, "AdaptiveTrigger") => null,
			(NSPresentation, "ColumnDefinition") => null,
			(NSPresentation, "RowDefinition") => null,

			_ => throw new NotImplementedException($"Localization properties for are `${e.GetNiceName()}`({e.Name.NamespaceName}) not defined."),
		};
		
		IEnumerable<LocalizableProperty> GetDefinedAttributes(params XName[] names)
		{
			return names
				.Select(x => e.Attribute(x))
				.Where(x => x != null)
				.Where(IsLocalizable)
				.Select(x => new LocalizableProperty(x.Name.LocalName, x.Value));
		}
		IEnumerable<LocalizableProperty> SelectAttributes(Func<XAttribute, bool> predicate)
		{
			return e.Attributes()
				.Where(predicate)
				.Where(IsLocalizable)
				.Select(x => new LocalizableProperty(x.Name.LocalName, x.Value));
		}
		
		bool IsLocalizable(XAttribute attribute)
		{
			return
				attribute.Value.StartsWith("{}") ||
				!attribute.Value.StartsWith("{");
		}
	}
}
public record LocalizableProperty(string Name, string Value);

public static class XElementExtensions
{
	public static string PrettyPrint(this XElement e)
	{
		XmlWriterSettings settings = new XmlWriterSettings();
		settings.Indent = true;
		settings.OmitXmlDeclaration = true;
		settings.NewLineOnAttributes = true;
		
		var buffer = new StringBuilder();
		using (var writer = XmlWriter.Create(buffer, settings))
		{
			e.WriteTo(writer);
		}
		
		return buffer.ToString();
	}
	public static string GetNiceName(this XElement e)
	{
		return e.Name.Namespace != e.GetDefaultNamespace()
			? e.GetPrefixOfNamespace(e.Name.Namespace) + ":" + e.Name.LocalName
			: e.Name.LocalName;
	}
}

public static class Clickable
{
    public static Hyperlinq Create(string header, Action action) => new Hyperlinq(action, header);

    public static Hyperlinq CopyText(string text) => CopyText(text, text);
    public static Hyperlinq CopyText(string header, string text)
    {
        return Create(header, () => Clipboard.SetText(text));
    }
    //    public static Hyperlinq OpenInVisualStudio(string header, string path)
    //    {
    //        return Create(header, () => Util.Cmd(@"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\Common7\IDE\devenv.exe", $"-edit {path}"));
    //    }
    //    public static Hyperlinq OpenInVSCode(string header, string path)
    //    {
    //        return Create(header, () => Util.Cmd(@"code", $"-r {path}"));
    //    }
}
