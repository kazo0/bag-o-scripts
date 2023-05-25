<Query Kind="Program" />

#define SKIP_NOTIMPLEMENTED_OBJECT // unimportant element for this project (should not be copied over when borrowing the code)
#define SKIP_ALL_NOTIMPLEMENTED_OBJECT // comment out when debugging
#define REPLACE_UNO_PLATFORM_XMLNS // consider platform-specific xmlns, like 'ios:', as the xaml default namespace
#define CONTINUE_ON_DUPLICATED_RES_KEY
#define SIMPLIFY_VALUES // {StaticResource MyRes} -> *MyRes

using static UserQuery.Global;
using static UserQuery.ValueSimplifier;

public static class Global
{
	// prefix
	public static readonly XNamespace X = XNamespace.Get(NSX);
	public static readonly XNamespace Presentation = XNamespace.Get(NSPresentation);

	// xmlns (needed for switch statement)
	public const string NSX = "http://schemas.microsoft.com/winfx/2006/xaml";
	public const string NSPresentation = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
}

void Main() => DebugVisualState(
	//@"C:\Program Files (x86)\Windows Kits\10\DesignTime\CommonConfiguration\Neutral\UAP\10.0.22000.0\Generic\generic.xaml", 
	//new ResourceKey(TargetType: "ToggleSwitch")
	@"D:\code\uno\framework\Uno\src\Uno.UI.FluentTheme.v2\Resources\Version2\PriorityDefault\CheckBox_themeresources.xaml",
	"DefaultCheckBoxStyle"
	//new ResourceKey(TargetType: "CheckBox") // can use xmlns localname here, eg: utu:Control
);

void DebugVisualState(string path, ResourceKey key)
{
	var rd = ScuffedXamlParser.Load<ResourceDictionary>(path).Dump("ResourceDictionary", 0);
	var style = rd[key]?.As<Style>().Dump($"Style: {key}", 0);
	var vsgs = style
		?.Setters.FirstOrDefault(x => x.Property == "Template")?.Value.As<ControlTemplate>()
		?.TemplateRoot.AttachedProperties["VisualStateManager.VisualStateGroups"]?.As<object[]>()
		?.Cast<VisualStateGroup>()
		.ToArray();

	vsgs.SelectMany(vsg => vsg.VisualStates.Select(vs => new { VisualStateGroup = vsg.Name, VisualState = vs.Name, vs.Storyboard, vs.Setters }))
		.Dump("VisualStates", 0);
	vsgs.SelectMany(vsg => vsg.Transitions.Select(t => new { VisualStateGroup = vsg.Name, t.Name, t.From, t.To, t.Storyboard }))
		.Dump("VisualTranstions", 0);

	vsgs
		.SelectMany(vsg => new[]
			{
				vsg.VisualStates.SelectMany(vs => vs.Setters.Select(x => new { Source = vs.Name, x.Target, x.Value })),
				vsg.VisualStates.SelectMany(vs => (vs.Storyboard?.Children).Safe().Select(x => new { Source = vs.Name, Target = x.TargetName + x.TargetProperty?.Prefix("."), Value = (object)x.Value })),
				vsg.Transitions.SelectMany(t => (t.Storyboard?.Children).Safe().Select(x => new { Source = $"{t.From}->{t.To}", Target = x.TargetName + x.TargetProperty?.Prefix("."), Value = (object)x.Value })),
			}
			.SelectMany(row => row.Safe().Select(x => new { Source = $"{vsg.Name}\\{x.Source}", x.Target, x.Value }))
		)
		.Select(x => new { x.Target, x.Source, x.Value })
		.OrderBy(x => x.Target).ThenBy(x => !x.Source.Contains("->")).ThenBy(x => x.Source)
		.Dump("All Flattened", 1);
}

public static class ScuffedXamlParser
{
	public static readonly string[] IgnoredXmlnsPrefixes = "todo,void".Split(',');

	public static T Load<T>(string path)
	{
		Util.Metatext($"Processing: {path}").Dump();
		var xaml = File.ReadAllText(path);
		
		return (T)ScuffedXamlParser.Parse(XElement.Parse(xaml));
	}
	public static object Parse(XElement e)
	{
		if (SimpleValueParser.TryParseKnownTypes(e, out var value))
		{
			return value;
		}

		if (IsExplicitlyIgnored(e))
		{
			return new IgnoredXamlObject();
		}

		if (VisualTreeParser.TryParseKnownTypes(e, out var vte))
		{
			return vte;
		}

		return e.GetNameParts() switch
		{
			(NSPresentation, nameof(ResourceDictionary)) => ResourceDictionary.Parse(e),
			(NSPresentation, nameof(Style)) => Style.Parse(e),
			(NSPresentation, nameof(Setter)) => Setter.Parse(e),
			(NSPresentation, nameof(ControlTemplate)) => ControlTemplate.Parse(e),
			(NSPresentation, nameof(VisualStateGroup)) => VisualStateGroup.Parse(e),
			(NSPresentation, nameof(VisualTransition)) => VisualTransition.Parse(e),
			(NSPresentation, nameof(VisualState)) => VisualState.Parse(e),
			
			(NSPresentation, "DataTemplate") => null,

			_ => UnhandledResult(),
		};
		
		object UnhandledResult()
		{
#if SKIP_ALL_NOTIMPLEMENTED_OBJECT
			Util.Metatext($"Skipped unknown type: {e.Name.ToString()}").Dump();
			return null;
#else
			throw new NotImplementedException(e.Name.ToString()),
#endif
		}
	}

	// we dont actually want to just take Ignorable as is (due to uno specific xmlns), so we will use a custom list here
	// we dont want to introduce the notion of "platform" yet...
	public static bool IsExplicitlyIgnored(XElement e)
	{
		var prefix = e.GetPrefixOfNamespace(e.Name.NamespaceName);
		return IgnoredXmlnsPrefixes.Contains(prefix);
	}
	public static bool IsExplicitlyIgnored(XAttribute attribute)
	{
		var prefix = attribute.Parent.GetPrefixOfNamespace(attribute.Name.NamespaceName);
		return IgnoredXmlnsPrefixes.Contains(prefix);
	}
}
public static class SimpleValueParser // for primitive, struct, poco types
{
	private static readonly string[] DontCareTypes = @"
			String, Double
			Thickness, CornerRadius
			StaticResource, ThemeResource
		".Split("\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			.SelectMany(x => x.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
			.ToArray();

	// this method should only be used by ScuffedXamlParser
	// return false for unhandled types, throw when failing to parse known type
	public static bool TryParseKnownTypes(XElement e, out object result)
	{
		var (xmlns, name) = e.GetNameParts();

		if (ScuffedXamlParser.IsExplicitlyIgnored(e))
		{
			result = new IgnoredXamlObject();
			return true;
		}
		if (DontCareTypes.Contains(name))
		{
			result = new DontCareXamlObject();
			return true;
		}

		(var success, result) = (xmlns, name) switch
		{
			_ => (false, default(object)),
		};
		return success;
	}

	private static IgnoredXamlObject Ignored(XElement e) => new();
}
public static class VisualTreeParser // generic parser for all visual tree elements or DependencyObjects
{
	private static readonly string[] WhiteListedTypes = @"
			Border, Control, ContentControl, ContentPresenter, ScrollViewer, Button, Flyout
			Grid, StackPanel, StackLayout
			ItemsRepeater, ItemsRepeaterScrollHost
			SplitView, 
			NavigationView, NavigationViewItem, NavigationViewItemSeparator,
			ElevatedView
			// DependencyObjects... just lumping them here for now
			RowDefinition, ColumnDefinition
		".Split("\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			.Where(x => !x.StartsWith("//"))
			.SelectMany(x => x.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
			.ToArray();

	// this method should only be used by ScuffedXamlParser
	// return false for unhandled types, throw when failing to parse known type
	public static bool TryParseKnownTypes(XElement e, out VisualTreeElement result)
	{
		if (WhiteListedTypes.Contains(e.Name.LocalName))
		{
			result = VisualTreeElement.Parse(e);
			return true;
		}

		result = default;
		return false;
	}
}
public static class XamlObjectHelper
{
	public static bool IsCollectionType(XElement e)
	{
		return e.GetNameParts() switch
		{
			(_, "VisualStateManager.VisualStateGroups") => true,
			(_, "Grid.RowDefinitions" or "Grid.ColumnDefinitions") => true,

			_ => false,
		};
	}
}
public static class ValueSimplifier
{
	private static bool ShouldSimplify(string value)
	{
#if SIMPLIFY_VALUES
		return !string.IsNullOrEmpty(value);
#else
		return false;
#endif
	}
	
	public static string SimplifyMarkup(string value)
	{
		if (!ShouldSimplify(value)) return value;
		
		if (Regex.Match(value, @"^{(Static|Theme)Resource (?<key>\w+)}$") is { Success: true } resourceMarkup)
		{
			return resourceMarkup.Result("[${key}]");
		}
		// fixme: naive parser
		if (Regex.Match(value, "{Binding(?<vargs>.+)?}") is { Success: true } bindingMarkup)
		{
			return bindingMarkup.Groups["vargs"].Value.Split(',')
				.Select(x => x.Trim().Split('=', 2))
				.ToDictionary(x => x.Length > 1 ? x[0] : "Path", x => x.Last())
				.TryGetValue("Path", out var path) 
					? $"{{{path}}}" : "{this}";
		}
		
		return value;
	}
	
	public static string SimplifyTimeSpan(string value)
	{
		if (!ShouldSimplify(value)) return value;
		
		return Regex.Replace(value, "^(00?:)+", "");
	}
}

// xaml object models
public interface IScriptIgnorable { }
public record IgnoredXamlObject() : IScriptIgnorable; // for todo:, void:
public record DontCareXamlObject() : IScriptIgnorable; // xaml object that we couldnt be bothered to parse
public record ResourceKey(string Key = null, string TargetType = null) : IComparable
{
	public static implicit operator ResourceKey(string key) => new(key);

	public int CompareTo(object obj)
	{
		if (obj is ResourceKey other)
		{
			int? Compare(string a, string b) => (a, b) switch
			{
				(null, null) => null,
				(_, null) => 1,
				(null, _) => -1,
				(_, _) => a.CompareTo(b),
			};

			return Compare(TargetType, other.TargetType) ?? Compare(Key, other.Key) ?? 0;
		}

		return 1;
	}

	public override string ToString() => Key?.ToString() ?? $"TargetType={TargetType}";
	private object ToDump() => ToString();
}
public class ResourceDictionary : Dictionary<ResourceKey, object>
{
	public void Add(object resource)
	{
		throw new NotImplementedException();
		/*if (!TryAdd(resource.Key, resource))
		{
#if ALLOW_DUPLICATED_KEYS
#if !ALLOW_DUPLICATED_KEYS_WITHOUT_WARNING
			Util.WithStyle($"Duplicated key: {resource.Key}", "color: orange").Dump();
#endif
#else
			throw new ArgumentException($"Duplicated key: {resource.Key}");
#endif
		}*/
	}
	public void Merge(ResourceDictionary other)
	{
		foreach (var kvp in other)
			Add(kvp.Key, kvp.Value);
	}

	// public object ToDump() => Values;

	public static ResourceKey GetResourceKey(XElement e)
	{
		// https://docs.microsoft.com/en-us/windows/apps/design/style/xaml-resource-dictionary
		// x:Name can be used instead of x:Key. However, [...less optimal].
		string key =
			e.Attribute(X + "Key")?.Value ??
			e.Attribute(X + "Name")?.Value;

		// the key can also be missing, in case of implicit style
		if (key == null && e.Name.Is<Style>() && e.Attribute("TargetType")?.Value is { Length: > 0 } type)
		{
			return new(null, TargetType: type);
			// note: there is also keyed + typed style, but in that case only the key will be used for lookup
		}

		return new(key ?? throw new Exception("Key not found on element."));
	}

	public static ResourceDictionary Parse(XElement e)
	{
		var result = new ResourceDictionary();
		foreach (var child in e.Elements())
		{
			if (child.Name.IsMemberOf<ResourceDictionary>(out var property))
			{
				_ = property switch
				{
					"ThemeDictionaries" => default(object),
					"MergedDictionaries" => default(object),

					_ => throw new NotImplementedException(property),
				};
			}
			else
			{
				var key = ResourceDictionary.GetResourceKey(child);
				var value = ScuffedXamlParser.Parse(child);

				if (value is not IScriptIgnorable)
				{
					if (!result.TryAdd(key, value))
					{
						var exception = new ArgumentException($"An item with the same key has already been added. {key}");
#if !CONTINUE_ON_DUPLICATED_RES_KEY
						throw exception;
#else
						exception.Dump();
#endif
					}
				}
			}
		}

		return result;
	}

	public object this[string Key = null, string TargetType = null]
	{
		get => this.TryGetValue(new(Key, TargetType), out var value) ? value : default;
	}
}
public record Style(string BasedOn = null, string TargetType = null)
{
	public List<Setter> Setters { get; } = new();

	public static Style Parse(XElement e)
	{
		var result = new Style(e.Attribute("BasedOn")?.Value, e.Attribute("TargetType")?.Value);

		foreach (var child in e.Elements())
		{
			var item = ScuffedXamlParser.Parse(child);
			if (item is IScriptIgnorable) continue;

			if (item is Setter setter)
			{
				result.Setters.Add(setter);
			}
			else
			{
				throw new NotImplementedException($"Style > {child.Name.Pretty()}").PreDump(child);
			}
		}

		return result;
	}
}
public record Setter(string Property, string Target, object Value)
{
	//public object ToDump() => $"{Property ?? Target} -> {Value}";

	public static Setter Parse(XElement e)
	{
		//var valueMember = e.Elements().FirstOrDefault(x => x.Name.IsMemberOf<Setter>(out var member) && member == "Value");
		//var value = valueMember?.Elements().FirstOrDefault();

		return new Setter
		(
			Property: e.Attribute("Property")?.Value,
			Target: e.Attribute("Target")?.Value,
			Value: GetDirectOrNestedValue()
		);

		object GetDirectOrNestedValue()
		{
			if (e.Element(Presentation + "Setter.Value") is { } valueMember && valueMember.HasElements)
			{
				return ScuffedXamlParser.Parse(valueMember.Elements().Single());
			}

			return e.Attribute("Value")?.Value;
		}
	}
}
public record ControlTemplate(string TargetType = null)
{
	public VisualTreeElement TemplateRoot { get; private set; }

	public static ControlTemplate Parse(XElement e)
	{
		var result = new ControlTemplate(e.Attribute("TargetType")?.Value);
		var elements = e.Elements().ToArray();
		result.TemplateRoot = elements.Length switch
		{
			0 => null,
			1 => (VisualTreeElement)ScuffedXamlParser.Parse(elements[0]),
			_ => throw new InvalidOperationException($"ControlTemplate > multiple children are present").PreDump(e),
		};

		return result;
	}
}

public record VisualTreeElement(string Type)
{
	public static readonly string[] SkippedMembers = @"
		RenderTransform
		".Split("\n", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
			.Where(x => !x.StartsWith("//"))
			.SelectMany(x => x.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
			.ToArray();

	public string Name { get; private set; }
	public Dictionary<string, object> Properties { get; private set; } = new();
	public Dictionary<string, object> AttachedProperties { get; private set; } = new();
	public List<VisualTreeElement> Children { get; private set; } = new();

	public static VisualTreeElement Parse(XElement e)
	{
		var result = new VisualTreeElement(e.Name.LocalName);
		foreach (var attribute in e.Attributes())
		{
			ParseAttributeMember(attribute, result);
		}
		foreach (var child in e.Elements())
		{
			ParseChildMember(child, result);
		}

		return result;
	}
	public static void ParseAttributeMember(XAttribute attribute, VisualTreeElement target)
	{
		if (ScuffedXamlParser.IsExplicitlyIgnored(attribute)) return;

		var (xmlns, name) = attribute.GetNameParts();
		if (xmlns == string.Empty)
		{
			xmlns = attribute.Parent.GetDefaultNamespace().NamespaceName;
		}

		switch ((xmlns, name))
		{
			case (NSPresentation, _): target.Properties[name] = attribute.Value; return;
			case (NSX, "Name"): target.Name = attribute.Value; return;
			case (_, _) when name.Contains('.'): target.AttachedProperties[name] = attribute.Value; return;

			default: //target.Properties.Add(name, attribute.Value); return;
				attribute.Dump((xmlns, name).ToString());
				throw new NotImplementedException();
		}
	}
	public static void ParseChildMember(XElement child, VisualTreeElement target)
	{
		if (ScuffedXamlParser.IsExplicitlyIgnored(child)) return;

		var (xmlns, name) = child.GetNameParts();
		if (xmlns == string.Empty)
		{
			xmlns = child.GetDefaultNamespace().NamespaceName;
		}

		/* we have 3 cases here: // for child element only (not applicable to attribute)
			1. member property whose local name is $"{Parent.ClassName or its upperclass name}.{PropertyName}"
			2. attached property whose local name is $"{DeclaringType}.{PropertyName}"
			3. any remaining should be direct content
		*/

		var parts = name.Split('.', 2);
		var className = parts[0];
		var memberName = parts.ElementAtOrDefault(1);
		if (memberName != null && target.IsMatchingClass(className)) // case 1
		{
			//child.Dump((xmlns, name).ToString());
			//throw new NotImplementedException();
			return;  // todo
		}
		else if (memberName != null) // case 2
		{
			if (ParseValue() is { } value && value is not IScriptIgnorable)
			{
				target.AttachedProperties[name] = ParseValue();
			}
			//child.Dump((xmlns, name).ToString());
			//throw new NotImplementedException();

			object ParseValue()
			{
				var elements = child.Elements().ToArray();
				if (XamlObjectHelper.IsCollectionType(child))
				{
					return elements.Select(ScuffedXamlParser.Parse).Where(x => x is not IScriptIgnorable).ToArray();
				}
				else
				{
					if (elements.Length == 0) return null;
					else if (elements.Length == 1) return ScuffedXamlParser.Parse(elements[0]);
					throw new Exception($"{child.Name} is not a collection type, but nests more than one element.");
				}
			}
		}
		else // case 3
		{
			return; // todo
			var parsed = ScuffedXamlParser.Parse(child);
			if (parsed is VisualTreeElement vte)
			{
				target.Children.Add((VisualTreeElement)ScuffedXamlParser.Parse(child));
			}
			else
			{
				child.Dump((xmlns, name).ToString());
				throw new NotImplementedException();
			}
		}
	}

	private bool IsMatchingClass(string className)
	{
		// todo: check through class hierarchy too
		return Type == className;
	}
}
public record VisualStateGroup(string Name)
{
	public List<VisualState> VisualStates { get; } = new();
	public List<VisualTransition> Transitions { get; } = new();

	public static VisualStateGroup Parse(XElement e)
	{
		var result = new VisualStateGroup(e.Attribute(X + "Name")?.Value);
		foreach (var child in e.Elements())
		{
			if (child.Name.Is<VisualState>())
			{
				result.VisualStates.Add(VisualState.Parse(child));
			}
			else if (child.Name.IsMemberOf<VisualStateGroup>(nameof(Transitions)))
			{
				result.Transitions.AddRange(ParseTransitions(child));
			}
			else
			{
				child.Dump();
				throw new NotImplementedException($"Unexpected {nameof(VisualStateGroup)} child element: {child.Name}");
			}
		}

		return result;
	}
	public static IEnumerable<VisualTransition> ParseTransitions(XElement transitions)
	{
		foreach (var child in transitions.Elements())
		{
			var item = ScuffedXamlParser.Parse(child);
			if (item is IScriptIgnorable) continue;
			if (item is VisualTransition transition)
			{
				yield return transition;
			}
			else
			{
				child.Dump();
				throw new NotImplementedException($"Unexpected {transitions.Name.LocalName} child element: {child.Name}");
			}
		}
	}
}
public record VisualState(string Name)
{
	public Storyboard Storyboard { get; private set; }
	public List<Setter> Setters { get; private set; } = new();

	public static VisualState Parse(XElement e)
	{
		var result = new VisualState(e.Attribute(X + "Name")?.Value);

		foreach (var child in e.Elements())
		{
			if (child.Name.IsMemberOf<VisualState>(out var memberName))
			{
				if (memberName == "Setters")
				{
					result.Setters.AddRange(ParseSetters(child));
				}
			}
			else if (child.Name.Is("Storyboard"))
			{
				result.Storyboard = Storyboard.Parse(child);
			}
			else
			{
				child.Dump();
				throw new NotImplementedException($"Unexpected {nameof(VisualState)} child element: {child.Name}");
			}
		}

		return result;
	}
	public static IEnumerable<Setter> ParseSetters(XElement setters)
	{
		foreach (var child in setters.Elements())
		{
			var item = ScuffedXamlParser.Parse(child);
			if (item is IScriptIgnorable) continue;
			if (item is Setter setter)
			{
				yield return setter;
			}
			else
			{
				child.Dump();
				throw new NotImplementedException($"Unexpected {setters.Name.LocalName} child element: {child.Name}");
			}
		}
	}
}
public record VisualTransition(string Name, string From, string To)
{
	public Storyboard Storyboard { get; private set; }

	public static VisualTransition Parse(XElement e)
	{
		var result = new VisualTransition(e.Attribute(X + "Name")?.Value, e.Attribute("From")?.Value, e.Attribute("To")?.Value);

		foreach (var child in e.Elements())
		{
			if (child.Name.Is("Storyboard"))
			{
				result.Storyboard = Storyboard.Parse(child);
			}
			else
			{
				child.Dump();
				throw new NotImplementedException($"Unexpected {nameof(VisualTransition)} child element: {child.Name}");
			}
		}

		return result;
	}
}

public record Timeline(string Type)
{
	public string TargetName { get; private set; }
	public string TargetProperty { get; private set; }
	public string Value { get; private set; }
	//public string ValueTimeline { get; private set; }

	public static Timeline Parse(XElement e)
	{
		return e.GetNameParts() switch
		{
			(_, "ColorAnimation") => ParseSimpleAnimation(),
			(_, "ColorAnimationUsingKeyFrames") => ParseKeyFrames(),
			(_, "DoubleAnimation") => ParseSimpleAnimation(),
			(_, "DoubleAnimationUsingKeyFrames") => ParseKeyFrames(),
			(_, "DragItemThemeAnimation") => ParsePreconfiguredAnimation(),
			(_, "DragOverThemeAnimation") => ParsePreconfiguredAnimation(),
			(_, "DrillInThemeAnimation") => ParsePreconfiguredAnimation(),
			(_, "DrillOutThemeAnimation") => ParsePreconfiguredAnimation(),
			(_, "DropTargetItemThemeAnimation") => ParsePreconfiguredAnimation(),
			(_, "FadeInThemeAnimation") => ParsePreconfiguredAnimation(),
			(_, "FadeOutThemeAnimation") => ParsePreconfiguredAnimation(),
			(_, "ObjectAnimationUsingKeyFrames") => ParseKeyFrames(),
			(_, "PointAnimation") => ParseSimpleAnimation(),
			(_, "PointAnimationUsingKeyFrames") => ParseKeyFrames(),
			(_, "PointerDownThemeAnimation") => ParsePreconfiguredAnimation(),
			(_, "PointerUpThemeAnimation") => ParsePreconfiguredAnimation(),
			(_, "PopInThemeAnimation") => ParsePreconfiguredAnimation(),
			(_, "PopOutThemeAnimation") => ParsePreconfiguredAnimation(),
			(_, "RepositionThemeAnimation") => ParsePreconfiguredAnimation(),
			(_, "SplitCloseThemeAnimation") => ParsePreconfiguredAnimation(),
			(_, "SplitOpenThemeAnimation") => ParsePreconfiguredAnimation(),
			//(_, "Storyboard") => Parse__(),
			(_, "SwipeBackThemeAnimation") => ParsePreconfiguredAnimation(),
			(_, "SwipeHintThemeAnimation") => ParsePreconfiguredAnimation(),

			_ => null,	
		};
		
		Timeline ParseCommon()
		{
			var result = new Timeline(e.Name.LocalName);
			result.TargetName = e.Attribute("Storyboard.TargetName")?.Value;
			result.TargetProperty = e.Attribute("Storyboard.TargetProperty")?.Value;
		
			//result.Dump(); e.Dump(); throw new NotImplementedException();
			return result;
		}
		Timeline ParseKeyFrames()
		{
			var result = ParseCommon();
			result.Value = string.Join("\n", e.Elements().Select(ParseKeyFrame));
			
			return result;
		}
		string ParseKeyFrame(XElement frame)
		{
			string Value(string key = "Value") => SimplifyMarkup(frame.Attribute(key)?.Value ?? frame.GetMember("Value").Value);
			string KeyTime(string key = "KeyTime") => SimplifyMarkup(SimplifyTimeSpan(frame.Attribute(key)?.Value));
			string Raw(string key) => frame.Attribute(key)?.Value;
			
			return frame.Name.LocalName switch
			{
				"DiscreteObjectKeyFrame" => $"{Value()} @{KeyTime()}",
				
				// DoubleKeyFrame
				"DiscreteDoubleKeyFrame" => $"{Value()} @{KeyTime()}",
				"EasingDoubleKeyFrame" => $"{Value()} @{KeyTime()} f={frame.Attribute("EasingFunction")}",
				"LinearDoubleKeyFrame" => $"{Value()} @{KeyTime()} f=Linear",
				"SplineDoubleKeyFrame" => $"{Value()} @{KeyTime()} f=Spline",
				
				"LinearColorKeyFrame" => $"{Value()} @{KeyTime()} f=Linear",
				
				_ => throw new NotImplementedException($"{e.Name.LocalName} > {frame.Name.Pretty()}").PreDump(frame),
			};
		}
		Timeline ParseSimpleAnimation()
		{
			var result = ParseCommon();
			result.Value = e.Name.LocalName switch
			{
				"DoubleAnimation" => FormatAnimation(),
				"ColorAnimation" => FormatAnimation(),
				
				_ => throw new NotImplementedException(e.Name.Pretty()).PreDump(e),
			};
			
			return result;
			
			string FormatAnimation()
			{
				var from = SimplifyMarkup(e.Attribute("From")?.Value);
				var by = SimplifyMarkup(e.Attribute("By")?.Value);
				var to = SimplifyMarkup(e.Attribute("To")?.Value);
				var duration = SimplifyMarkup(SimplifyTimeSpan(e.Attribute("Duration")?.Value));
				
				return (from, by, to) switch
				{
					(_, null, null) => $"{from}->$base in {duration}",
					(null, _, null) => $"$current->{by} in {duration}",
					(null, null, _) => $"$current->{to} in {duration}",
					(_, null, _) => $"{from}->{to} in {duration}",
					(_, _, null) => $"{from}->{by} in {duration}",

					// -- https://learn.microsoft.com/en-us/uwp/api/windows.ui.xaml.media.animation.doubleanimation?view=winrt-22621#remarks
					_ => throw new InvalidOperationException($"A DoubleAnimation typically has at least one of the From, By or To properties set, but never all three: from={from}, by={by}, to={to}"),
				};
			}
		}
		Timeline ParsePreconfiguredAnimation()
		{
			var result = ParseCommon();
			result.TargetName ??= e.Attribute("TargetName")?.Value;
			result.Value = result.Type;
			
			return result;
		}
	}
}
public record Storyboard // doesnt make sense to inherit from Timeline here (while it was the case in uwp)
{
	public List<Timeline> Children { get; } = new();
	
	public static Storyboard Parse(XElement e)
	{
		var result = new Storyboard();
		foreach (var child in e.Elements())
		{
			if (Timeline.Parse(child) is { } timeline)
			{
				result.Children.Add(timeline);
			}
			else
			{
				throw new NotImplementedException($"Storyboard > {child.Name.Pretty()}");
			}
		}

		return result;
	}
	
	public object ToDump() => Children;
}

// extensions
public static class StringExtensions
{
	public static string Prefix(this string value, string prefix) => prefix + value;
}
public static class FluentExtensions
{
	public static T As<T>(this object x) where T : class => x as T;
}
public static class EnumerableExtensions
{
	public static IEnumerable<T> Safe<T>(this IEnumerable<T> source) => source is not null ? source : Array.Empty<T>();
}
public static class ExpcetionExtensions
{
	/// <summary>Used to dump an object inside a throw-expression</summary>
	public static TException PreDump<TException>(this TException exception, object dump) where TException: Exception
	{
		dump.Dump();
		return exception;
	}
}
public static class XNameExtensions
{
	public static (string Xmlns, string LocalName) GetNameParts(this XElement e, bool trimApiContract = true) => e.Name.GetNameParts(trimApiContract);
	public static (string Xmlns, string LocalName) GetNameParts(this XAttribute e, bool trimApiContract = true) => e.Name.GetNameParts(trimApiContract);
	public static (string Xmlns, string LocalName) GetNameParts(this XName name, bool trimApiContract = true)
	{
		var xmlns = name.NamespaceName;
		if (trimApiContract && xmlns.IndexOf('?') is var index && index > 0)
		{
			xmlns = xmlns[..(index)];
		}
#if REPLACE_UNO_PLATFORM_XMLNS
		if (xmlns.StartsWith("http://uno.ui/"))
		{
			xmlns = NSPresentation;
		}
#endif

		return (xmlns, name.LocalName);
	}
	public static string Pretty(this XName name)
	{
		var (xmlns, localName) = name.GetNameParts();

		return $"{{{xmlns}}}{localName}";
	}

	public static bool Match(this XName x, string xmlns, string localName) => x.GetNameParts() == (xmlns, localName);

	public static bool Is<T>(this XName x) => x.LocalName == typeof(T).Name;
	public static bool Is(this XName x, string name) => x.LocalName == name;
	public static bool IsAnyOf(this XName x, params string[] names) => names.Contains(x.LocalName);

	public static bool IsMemberOf<T>(this XName x) => x.IsMemberOf(typeof(T));
	public static bool IsMemberOf(this XName x, Type type) => x.IsMemberOf(type.Name);
	public static bool IsMemberOf(this XName x, string typeName) => x.IsMemberOf(typeName, out var _);
	public static bool IsMemberOf<T>(this XName x, string memberName) => x.IsMemberOf(typeof(T), memberName);
	public static bool IsMemberOf(this XName x, Type type, string memberName) => x.IsMemberOf(type.Name, memberName);
	public static bool IsMemberOf(this XName x, string typeName, string memberName) => x.LocalName == $"{typeName}.{memberName}";
	public static bool IsMemberOf<T>(this XName x, out string memberName) => x.IsMemberOf(typeof(T), out memberName);
	public static bool IsMemberOf(this XName x, Type type, out string memberName) => x.IsMemberOf(type.Name, out memberName);
	public static bool IsMemberOf(this XName x, string typeName, out string memberName)
	{
		memberName = x.LocalName.Split(".") is { Length: 2 } parts && parts[0] == typeName
			? parts[1]
			: default;

		return memberName != default;
	}

	public static XElement GetMember(this XElement e, string memberName) => e.Element(e.Name.Namespace + $"{e.Name.LocalName}.{memberName}");
}