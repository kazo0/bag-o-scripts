<Query Kind="Program">
  <Reference>&lt;RuntimeDirectory&gt;\WPF\PresentationCore.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\WPF\WindowsBase.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Xaml.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\WPF\UIAutomationTypes.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Configuration.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\WPF\System.Windows.Input.Manipulations.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\WPF\UIAutomationProvider.dll</Reference>
  <Reference>&lt;RuntimeDirectory&gt;\System.Deployment.dll</Reference>
  <Namespace>System.Windows</Namespace>
</Query>

#define USE_GET_ONLY_PROPERTY_SYNTAX // using new uwp/uno style, otherwise using legacy style with static field
//#define USE_CS71_DEFAULT_LITERAL (DONT USE THIS FOR NON-REF TYPE)
//#define USE_UNO_CORE_MAYBE_CAST // avoid using this
//#define USE_INSTANCED_HANDLER
//#define USE_INSTANCED_HANDLER_WITH_EXPLICIT_CAST
#define INDENT_USING_TAB
#define USE_UNO_IL_TRIM_OPTIMIZATION // to use in uno public libraries, or projects where applicable (note: only applicable to attached property)

void Main()
{
	var owner = "ItemsRepeaterExtensions"; // DP.OwnerType: class which this property is place under
	var control = "ItemsRepeater"; // class which this property can be set on; the least restrictive class is 'DependencyObject'

	var properties = new List<PropertyDefinition>
	{
		// syntax: new (string Name, string Type, string DefaultValue = default, bool? HasHandler = null); // for literal null value use "null"
		
		//new ("Localize", "string", null ),
		//new ("ApplyMinMaxBindingFix", "bool", "false" ),
		//new ("MaterialVisibleOn", "VisiblePlatforms", default ),
		//new ("ModelLanguage", "string", default),
		//new ("SomeEnumProperty", "MyEnum", default),
		
		//new ("IsSynchronizingSelection", "bool", HasHandler: false),
		//new ("SelectedItem", "object", HasHandler: true),
		//new ("SelectedItems", "IList<object>", HasHandler: true),
		//new ("SelectedIndex", "int", HasHandler: true),
		//new ("SelectedIndexes", "IList<int>", HasHandler: true),
		new ("SelectionMode", "ItemsSelectionMode", "(ItemsSelectionMode)0", HasHandler: true),
	};

	Util.AutoScrollResults = false;
	Util.Metatext("note: for uno control and uno-only (imported controls within uno.ui) use FrameworkPropertyMetadata instead of PropertyMetada; see uno#5984").Dump();

	DumpCode(string.Join("\n", properties
		.Select(x => owner == control
			? DependencyPropertyCodeGenerator.GenerateSimpleProperty(owner, x.Name, x.Type, x.DefaultValue, x.HasHandler == true)
			: DependencyPropertyCodeGenerator.GenerateAttachedProperty(owner, control, x.Name, x.Type, x.DefaultValue, x.HasHandler == true)
		)
	));
	if (properties.Where(x => x.HasHandler == true).ToArray() is { Length: > 0 } needHandlers)
	{
		DumpCode(string.Join("\n", needHandlers
			.Select(x => DependencyPropertyCodeGenerator.GenerateChangedHandler(control, x.Name, x.Type))
		));
	}

	void DumpCode(string source)
	{
		Console.WriteLine();
		Clickable.CopyText("Copy source", source).Dump();
		CodeSnippet.FromText(source).Dump();
	}
}

// Define other methods and classes here
public static class DependencyPropertyCodeGenerator
{
	public static string GenerateSimpleProperty(string owner, string name, string type, string defaultValue = null, bool withHandler = false)
	{
		var propertyMetadataParams = new[]
		{
			defaultValue ?? FormatDefaultValue(type),
			withHandler ? FormatPropertyChangedCallback(owner, name) : default
		};

		return $$"""
			#region DependencyProperty: {{name}}

			{{FormatDependencyProperty(name)}} = DependencyProperty.Register(
			    nameof({{name}}),
			    typeof({{type}}),
			    typeof({{owner}}),
			    new PropertyMetadata({{string.Join(", ", propertyMetadataParams.TrimNull())}}));

			public {{type}} {{name}}
			{
			    get => ({{type}})GetValue({{name}}Property);
			    set => SetValue({{name}}Property, value);
			}

			#endregion
			""".FormatIndentation();
	}
	public static string GenerateAttachedProperty(string owner, string host, string name, string type, string defaultValue = null, bool withHandler = false)
	{
		var propertyMetadataParams = new[]
		{
			defaultValue ?? FormatDefaultValue(type),
			withHandler ? FormatPropertyChangedCallback(host, name) : default
		};
		string GetMarker(string accessor) =>
#if USE_UNO_IL_TRIM_OPTIMIZATION
			$"[DynamicDependency(nameof({accessor}{name}))]\n";
#else
			null;
#endif

		// DO: AbcProperty = DP.RegisterAttached("Abc"
		// DONT: Abc = DP.RegisterAttached(nameof(Abc)
		// This cause the property changed handler not being fired at all
		return $"""
			#region DependencyProperty: {name}

			{FormatDependencyProperty(name, attached: true)} = DependencyProperty.RegisterAttached(
				"{name}",
				typeof({type}),
				typeof({owner}),
				new PropertyMetadata({string.Join(", ", propertyMetadataParams.TrimNull())}));

			{GetMarker("Set")}public static {type} Get{name}({host} obj) => ({type})obj.GetValue({name}Property);
			{GetMarker("Get")}public static void Set{name}({host} obj, {type} value) => obj.SetValue({name}Property, value);

			#endregion
			""".FormatIndentation();
	}
	public static string GenerateChangedHandler(string host, string name, string type)
	{
#if USE_INSTANCED_HANDLER
			return $"private void On{name}Changed(DependencyPropertyChangedEventArgs e) => throw new NotImplementedException();";
#else
			return $"private static void On{name}Changed(DependencyObject sender, DependencyPropertyChangedEventArgs e) => throw new NotImplementedException();";
#endif
	}

	private static string FormatDependencyProperty(string name, bool attached = false)
	{
		var marker =
#if USE_UNO_IL_TRIM_OPTIMIZATION
			attached ? $"[DynamicDependency(nameof(Get{name}))] " : "";
#else
			"";
#endif

		return
#if USE_GET_ONLY_PROPERTY_SYNTAX
			$"public static DependencyProperty {name}Property {{ {marker}get; }}";
#else
			$"public static readonly DependencyProperty {name}Property";
#endif
	}
	private static string FormatDefaultValue(string type)
	{
		return
#if USE_CS71_DEFAULT_LITERAL
			"default";
#else
			$"default({type})";
#endif
	}
	private static string FormatPropertyChangedCallback(string host, string name)
	{
#if USE_INSTANCED_HANDLER
#if USE_INSTANCED_HANDLER_WITH_EXPLICIT_CAST
				return $"(s, e) => (({host})s).On{name}Changed(e)";
#else
				return $"(s, e) => (s as {host})?.On{name}Changed(e)";
#endif
#else
				return $"On{name}Changed";
#endif
	}

	private static string FormatIndentation(this string value)
	{
#if INDENT_USING_TAB
		return value.Replace("    ", "\t");
#else
		return value.Replace("\t", "    ");
#endif
	}
}

public record PropertyDefinition(string Name, string Type, string DefaultValue = default, bool? HasHandler = null);

public static class Clickable
{
	public static Hyperlinq Create(string header, Action action) => new Hyperlinq(action, header);

	public static Hyperlinq CopyText(string text) => CopyText(text, text);
	public static Hyperlinq CopyText(string header, string text)
	{
		return Create(header, () => Clipboard.SetText(text));
	}
}
public static class CodeSnippet
{
	public static object FromText(string code) => Util.RawHtml(new XElement("pre", new XElement("code", code)));
}
public static class EnumerableExtensions
{
	public static IEnumerable<T> TrimNull<T>(this IEnumerable<T> source) where T : class
	{
		return source.Where(x => x is not null);
	}
}