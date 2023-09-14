//#define THROW_ON_NOTIMPLEMENTED_OBJECT
#define ENABLE_GENERIC_VALUE_OBJECT_PARSING
#define REPLACE_UNO_PLATFORM_XMLNS
#define ALLOW_DUPLICATED_KEYS // temp workaround for platform specifics
#define ALLOW_DUPLICATED_KEYS_WITHOUT_WARNING

public class Script
{
	public static void Main()
	{
		ResourceDictionary.ThemeMapping = new Dictionary<string, string>
		{
			["Light"] = "Light",
			["Dark"] = "Default,Dark" // Default=Dark is a weird concept introduced by lightweight styling...
		};

		//Specialized.ListExposedThemeV2Styles();
		//Specialized.ListExposedCupertinoStyles();
		//Specialized.ListExposedToolkitV2Styles();
		Specialized.DiffThemeV2InnerResources();
		//Specialized.CheckLightWeightResourceParity(@"D:\code\uno\platform\Uno.Themes\src/library/Uno.Material/Styles/Controls/v2/NavigationView.xaml");

		/*var additionalResources = new[]
		{
			@"D:\code\uno\framework\Uno\src\Uno.UI\UI\Xaml\Style\Generic\SystemResources.xaml",
			@"D:\code\uno\framework\Uno\src\Uno.UI.FluentTheme.v2\themeresources_v2.xaml",
			@"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\Styles\Application\Common\TextBoxVariables.xaml",
			@"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\Styles\Application\Common\Fonts.xaml",
			@"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\Styles\Application\v2\Typography.xaml",
			@"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\Styles\Application\v2\SharedColors.xaml",
			@"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\Styles\Application\v2\SharedColorPalette.xaml",
			@"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2\_Resources.xaml",
			@"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2\TextBlock.xaml"
		}.Aggregate(new ResourceDictionary(), (acc, file) => acc.Merge((ResourceDictionary)ScuffedXamlParser.Load(file)));*/
		//Specialized.CheckLightWeightResourceParity(@"D:\code\uno\platform\Uno.Themes\src/library/Uno.Material/Styles/Controls/v2/ToggleSwitch.xaml");
		//Specialized.ExtractLightWeightResources(@"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2\Button.xaml", additionalResources);
		//Specialized.ExtractLightWeightResources(@"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2\Slider.xaml", additionalResources);
		//string.Join("\n\n", Directory.GetFiles(@"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2\", "*.xaml")
		//	.Prepend(@"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\Styles\Application\v2\Typography.xaml")
		//	.Select(x => string.Join("\n", $"# {Path.GetFileName(x)}", Specialized.ExtractLightWeightResources(x, additionalResources)))
		//).OnDemand("Click to expand").Dump("All in one");
		//foreach (var control in Directory.GetFiles(@"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2\", "*.xaml"))
		//	Specialized.ExtractLightWeightResources(control, additionalResources);

		//ListColors(@"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\Styles\Application\v1\ColorPalette.xaml");
		//ListColors(@"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\Styles\Application\v2\SharedColorPalette.xaml");
		//ListColors(@"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\Styles\Application\v2\SharedColors.xaml");
		////ListColors(@"D:\code\uno\platform\Uno.Todo\src\ToDo.UI\Styles\ColorPaletteOverride.xaml");
		//ListColors(@"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\Styles\Application\v1\MaterialColors.xaml");
		/*SpecializedListColorTheme(
			@"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\Styles\Application\v2\SharedColorPalette.xaml",
			@"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\Styles\Application\v2\SharedColors.xaml",
			generateBrushesBasedOnColorAndOpacity: false);*/

		//foreach (var control in Directory.GetFiles(@"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2\", "*.xaml").Where(x => !Path.GetFileName(x).Contains('_')))
		//	ListStyles(control.Dump());

		//ListStyles(@"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\Styles\Controls\v1\Button.xaml");
		//ListStyles(@"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2\FloatingActionButton.xaml");
		//ListStyles(@"D:\code\uno\platform\Uno.UI.Toolkit\src\library\Uno.Toolkit.Material\Styles\Controls\v2\ChipGroup.xaml");
		//CompareStyles(@"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\Styles\Controls\v1\TextBlock.xaml", x => x.BasedOn != null);
		//CompareStyles(@"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2\TextBlock.xaml", x => true || x.BasedOn != null, x => Regex.Replace(x, @"^{(Static|Theme)Resource (?<key>\w+)}$", "(*${key})"));
		//CompareStyles(@"D:\code\uno\platform\Uno.Toolkit\src\library\Uno.Toolkit.Material\Styles\Controls\v2\Chip.xaml", x => x.BasedOn != null, SimplifyReference);
		//CompareStyles(@"D:\code\uno\platform\Uno.Toolkit\src\library\Uno.Toolkit.Material\Styles\Controls\v2\ChipGroup.xaml", x => x.BasedOn != null, SimplifyReference);
		/*var typography = (ResourceDictionary)ScuffedXamlParser.Load(@"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\Styles\Application\v2\Typography.xaml").Dump("Typography", 0);
		CompareStyles(@"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2\TextBlock.xaml", x => true || x.BasedOn != null);
		CompareStyles(@"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2\TextBlock.xaml", 
			x => true || x.BasedOn != null,
			resolveResource: x => ResolveResource(typography, x, 3)
		);*/

		Match TryMatch(string input, string pattern) => Regex.Match(input, pattern) is { Success: true } m ? m : null;
		string SimplifyReference(string value) => TryMatch(value, @"^{StaticResource (?<key>\w+)}$")?.Result("*${key}") ?? value;
		string ResolveResource(ResourceDictionary resources, string value, int maxDepth = 1)
		{
			for (int depth = 0; depth < maxDepth && value != null && TryMatch(value, @"^{StaticResource (?<key>\w+)}$") is { Success: true } match; depth++)
			{
				value = match.Groups["key"].Value.Apply(y => (resources[y] as StaticResource)?.Value?.ToString());
			}
			return value;
		}
	}

	private static void ListColors(string path)
	{
		Util.Metatext($"==================== {path}").Dump();
		var document = XDocument.Load(path);
		var root = (ResourceDictionary)ScuffedXamlParser.Parse(document.Root);

		object FormatColor(Color x) => Util.HorizontalRun(true, x.ToColoredBlock(), x.ToRgbText());

		root.Values
			.Where(x => x.IsReferenceFor<Color>())
			.Select(x => x switch
			{
				StaticResource sr => new { x.Key, Value = FormatColor((Color)sr.Value) },
				ThemeResource tr => new { x.Key, LightValue = FormatColor((Color)tr.LightValue), DarkValue = FormatColor((Color)tr.DarkValue) },

				_ => (object)null,
			})
			.Dump("Colors", 0);
		root.Values
			.Where(x => x.IsReferenceFor<Double>())
			.Dump("Doubles", 0);
		var brushes = root.Values
			.Where(x => x.IsStaticResourceFor<SolidColorBrush>())
			.Select(x => new { x.Key, Value = (x as StaticResource).Value as SolidColorBrush })
			.Select(x => new
			{
				x.Key,
				//Color = IKeyedResource.GetKeyFromMarkup(x.Value.GetDP("Color")),
				//Opacity = IKeyedResource.GetKeyFromMarkup(x.Value.GetDP("Opacity")),
				Color = x.Value.GetDP("Color"),
				Opacity = x.Value.GetDP("Opacity"),
			})
			.OrderBy(x => x.Color)
			.ThenBy(x => x.Opacity)
			.Dump("Brushes", 0);

		brushes
			.GroupBy(x => x.Color, (g, k) => Util.OnDemand($"{g}[{k.Count()}]", () => k))
			.Dump("Brushes by Color", 0);
	}
	private static void SpecializedListColorTheme(string paletteFile, string brushFile, bool generateBrushesBasedOnColorAndOpacity = false)
	{
#if false
		var paletteRD = (ResourceDictionary)ScuffedXamlParser.Parse(XDocument.Load(paletteFile).Root);
		var brushRD = (ResourceDictionary)ScuffedXamlParser.Parse(XDocument.Load(brushFile).Root);

		var colors = paletteRD.Values
			.Where(x => x.IsReferenceFor<Color>())
			.Select(x => x switch
			{
				StaticResource sr => new { x.Key, Value = new[] { (Color)sr.Value } },
				ThemeResource tr => new { x.Key, Value = new[] { (Color)tr.LightValue, (Color)tr.DarkValue } },

				_ => throw new Exception(),
			})
			.Dump("Colors", 0);
		var opacities = paletteRD.Values
			.Where(x => x.IsReferenceFor<Double>())
			.Dump("Opacities", 0);
		var brushes = brushRD.Values
			.Where(x => x.IsReferenceFor<SolidColorBrush>())
			//.Select(x => new { x.Key, Value = (x as StaticResource).Value as SolidColorBrush })
			.Select(x => new { x.Key, Value = (x as ThemeResource).LightValue as SolidColorBrush }) // both light and default(dark) are just duplicated for lightweight styling
			.Select(x => new
			{
				x.Key,
				//Color = IKeyedResource.GetKeyFromMarkup(x.Value.GetDP("Color")),
				//Opacity = IKeyedResource.GetKeyFromMarkup(x.Value.GetDP("Opacity")),
				//ColorValue = paletteRD.TryGetValue(IKeyedResource.GetKeyFromMarkup(x.Value.GetDP("Color")), out var color) ? color : default,
				//OpacityValue = paletteRD.TryGetValue(IKeyedResource.GetKeyFromMarkup(x.Value.GetDP("Opacity")), out var opacity) ? opacity : default,
			})
			.OrderBy(x => x.Color)
			.ThenBy(x => x.Opacity)
			.Dump("Brushes");

		colors.Select(color => new
		{
			BaseColor = color.Key,
			Brushes = from opacity in opacities.Prepend(null)
					  let key = color.Key.Key[0..^5] + opacity?.Key.Key[0..^7] + "Brush"
					  select new
					  {
						  Key = key,
						  Opacity = (opacity as StaticResource)?.Value as double? ?? 1,
						  Defined = Util.HighlightIf(brushes.Any(x => x.Key.Key == key), x => !x),
						  Copy = Util.HorizontalRun(true,
							  Clickable.CopyText("Key", key),
							  Clickable.CopyText("Ref", $"{{StaticResource {key}}}"),
							  Clickable.CopyText("Fwd", $"<StaticResource x:Key=\"\" ResourceKey=\"{key}\" />")
						  ),
					  }
		}).Dump("Quick Lookup", 0);

		if (generateBrushesBasedOnColorAndOpacity)
		{
			var crossProducts = (
				from color in colors
				from opacity in opacities.Prepend(null)
				let sanity0 = color.Key.Key.EndsWith("Color") ? true : throw new NotImplementedException()
				let sanity1 = opacity?.Key.Key.EndsWith("Opacity") != false ? true : throw new NotImplementedException()
				let key = color.Key.Key[0..^5] + opacity?.Key.Key[0..^7] + "Brush"
				select new
				{
					CK = color.Key,
					OK = opacity?.Key,
					Key = key,
					Defined = brushes.Any(x => x.Key.Key == key)
				}
			).ToArray();

			crossProducts.Dump("cross-products: Colors x Doubles", 0);
			crossProducts.GroupBy(x => x.CK).SelectMany(g => g.Select(x =>
				$@"<SolidColorBrush x:Key='{x.Key}'
					Color='{{ThemeResource {x.CK.Key}}}'
					{(x.OK.Apply(y => $"Opacity='{{StaticResource {y.Key}}}'"))}
				/>"
				.RegexReplace(@"\s+", " ")
				.Replace('\'', '"')
			)
				.Prepend($"<!--#region {g.Key.Key} -->")
				.Append($"<!--#endregion {g.Key.Key}-->")
			)
			.JoinBy("\n").Dump("all brushes");
			brushes.Select(x => x.Key.Key).Except(crossProducts.Select(x => x.Key)).Dump("missing brushes", 0);
		}
#endif
	}
	private static void ListStyles(string path)
	{
		var document = XDocument.Load(path);
		var root = (ResourceDictionary)ScuffedXamlParser.Parse(document.Root);

		root.Values
			.Select(x => (x as StaticResource)?.Value)
			.OfType<Style>()
			.Dump(path, 1);
	}
	private static void CompareStyles(string path, Func<Style, bool> predicate, Func<string, string> resolveResource = null)
	{
		// ^resolveResource: return null if unresolvable

		var document = XDocument.Load(path);
		var root = (ResourceDictionary)ScuffedXamlParser.Parse(document.Root);

		var styles = root.Values
			.Select(x => (x as StaticResource)?.Value)
			.OfType<Style>()
			.Where(predicate);

		PivotHelper.Pivot(styles,
			x => x.Key,
			x => x.Setters.ToDictionary(
				x => x.Property,
				x => (object)x.Value?.Apply(resolveResource ?? (_ => null)) ?? x.Value
			)
		).Dump(path);
	}

	private static partial class Specialized
	{
		public static void ListExposedThemeV2Styles()
		{
			var resources = new ResourceDictionary();
			var controls = Directory.GetFiles(@"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2\", "*.xaml")
				.Where(x => !Path.GetFileName(x).Contains('_'));
			foreach (var control in controls)
			{
				//Util.Metatext($"Processing: {control}").Dump();
				var filename = Path.GetFileNameWithoutExtension(control);
				var document = XDocument.Load(control);
				var root = (ResourceDictionary)ScuffedXamlParser.Parse(document.Root);

				var filter = (filename switch
				{
					"ContentDialog" => "Button",
					"DatePicker" => "Button",
					"NavigationView" => "Button,SplitView,TextBlock,ContentControl",
					"PasswordBox" => "Button",
					"PipsPager.Base" => "Button",
					"Slider" => "Thumb",
					"TextBox" => "Button",

					_ => "",
				});
				var filtered = root.Values
					.Where(x =>
						x is StaticResource { Value: Style { Key: string key, TargetType: string type } } &&
						!filter.Split(',').Contains(type)
					);
				//filtered
				//	.OfStaticResourceType<Style>()
				//	.Select(x => new { x.Key, x.TargetType })
				//	.Dump($"{Path.GetFileNameWithoutExtension(control)}: {filtered.OfStaticResourceType<Style>().Count()} styles", 0);

				resources.AddRange(filtered);
			}

			var _resources = (ResourceDictionary)ScuffedXamlParser.Load(@"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2\_Resources.xaml");
			var implicitStyles = _resources.Values.OfStaticResourceType<Style>()
				.Select(x => x.BasedOn)
#if true // Themes 3.0
				.Select(x => ((resources[x] as StaticResource)?.Value as Style)?.BasedOn)
#endif
				.ToArray()
				.Dump("ImplicitStyles", 0);
			var aliasMap = _resources.Values.Where(x => x.IsStaticResourceFor<StaticResourceRef>())
				.Select(x => new { x.Key, Key2 = ((x as StaticResource)?.Value as StaticResourceRef)?.Key })
				.ToDictionary(x => x.Key2, x => x.Key)
				.Dump("Aliases", 0);

			resources.Values.OfStaticResourceType<Style>()
				.Where(x => x.Key != null)
				.Where(x => x.Key is string k && !@"
						MaterialDefault, MaterialBase, 
						BaseStyle, BaseMaterial, BaseTextBlockStyle,
						MUX_
					".Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
					.SelectMany(x => x.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
					.Any(k.Contains)
				)
				.Select(x => new
				{
					x.Key,
					AliasedKey = aliasMap.TryGetValue(x.Key, out var key) ? key : "",
					x.TargetType,
					ImplicitStyle = implicitStyles.Contains(x.Key) ? "true" : "",
				})
				.Dump("=== Style Exports ===", 0)
				.ToCopyableMarkdownTable()
				.Dump();
		}
		/*not updated since uno5*/
		public static void ListExposedCupertinoStyles()
		{
			var styles = new ResourceDictionary();
			var controls = Directory.GetFiles(@"D:\code\uno\platform\Uno.Themes\src\library\Uno.Cupertino\Styles\Controls\", "*.xaml")
				.Where(x => !Path.GetFileName(x).Contains('_'));
			foreach (var control in controls)
			{
				//Util.Metatext($"Processing: {control}").Dump();
				var document = XDocument.Load(control);
				var root = (ResourceDictionary)ScuffedXamlParser.Parse(document.Root);

				styles.Merge(root);
			}

			//var styleInfos = ParseGetStyleInfos(@"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\MaterialResourcesV2.cs");
			styles.Values.OfStaticResourceType<Style>()
				.OrderBy(x => x.TargetType)
				//.Join(styleInfos, style => style.Key, info => info.ResourceKey, (style, info) => new { Style = style, Info = info })
				.Where(x => x.Key.StartsWith("Cupertino"))
				.Select(x => new
				{
					x.TargetType,
					Key = x.Key
				})
				//.OrderBy(x => x.TargetType)
				//.GroupBy(x => x.TargetType, (k, g) => $"`{k}`|" + string.Join("<br/>", g.Select(x => x.Key)))
				.Dump("CupertinoStyles", 0);
		}
		public static void ListExposedToolkitV2Styles()
		{
			var resources = new ResourceDictionary();
			var controls = Directory.GetFiles(@"D:\code\uno\platform\Uno.Toolkit\src\library\Uno.Toolkit.Material\Styles\Controls\v2\", "*.xaml")
				.Where(x => !Path.GetFileName(x).Contains('_'));

			foreach (var control in controls)
			{
				//Util.Metatext($"Processing: {control}").Dump();
				var filename = Path.GetFileNameWithoutExtension(control);
				var document = XDocument.Load(control);
				var root = (ResourceDictionary)ScuffedXamlParser.Parse(document.Root);

				var filter = (filename switch
				{
					"Chip" => "Button",
					"NavigationBar" => "utu:NavigationBarPresenter",
					"TabBar" => "utu:TabBarSelectionIndicatorPresenter",

					_ => "",
				});
				var filtered = root.Values
					.Where(x =>
						x is StaticResource { Value: Style { Key: string key, TargetType: string type } }
							? !filter.Split(',').Contains(type)
							: true
					);
				filtered
					.OfStaticResourceType<Style>()
					.Select(x => new { x.Key, x.TargetType })
					.Dump($"{Path.GetFileNameWithoutExtension(control)}: {filtered.OfStaticResourceType<Style>().Count()} styles", 0);

				resources.AddRange(filtered);
			}

			var _common = (ResourceDictionary)ScuffedXamlParser.Load(@"D:\code\uno\platform\Uno.Toolkit\src\library\Uno.Toolkit.Material\Styles\Controls\v2\_Common.xaml");
			var implicitStyles = _common.Values.OfStaticResourceType<Style>()
				.Select(x => x.BasedOn)
#if true // Toolkit 4.2
				//.Select(x => ((resources[x] as StaticResource)?.Value as Style)?.BasedOn)
#endif
				.ToArray()
				.Dump("ImplicitStyles", 0);
			var aliasMap = _common.Values.Where(x => x.IsStaticResourceFor<StaticResourceRef>())
				.Select(x => new { x.Key, Key2 = ((x as StaticResource)?.Value as StaticResourceRef)?.Key })
				.ToDictionary(x => x.Key2, x => x.Key)
				.Dump("Aliases", 0);

			resources.Values.OfStaticResourceType<Style>()
				.Where(x => x.Key != null)
				.Where(x => x.Key is string k && !@"
						MaterialDefault, MaterialBase, 
						BaseStyle, BaseMaterial, BaseTextBlockStyle,
					".Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
					.SelectMany(x => x.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
					.Any(k.Contains)
				)
				.Select(x => new
				{
					x.Key,
					AliasedKey = aliasMap.TryGetValue(x.Key, out var key) ? key : "",
					x.TargetType,
					ImplicitStyle = implicitStyles.Contains(x.Key) ? "true" : "",
				})
				.Dump("=== Style Exports ===", 0)
				.ToCopyableMarkdownTable()
				.Dump();
		}

		[Obsolete]
		private static IEnumerable<(string ResourceKey, string SharedKey, bool IsDefaultStyle)> GetThemeV2StyleInfos()
		{
			const string StylePrefix = "M3Material";
			var result = new List<(string ResourceKey, string SharedKey, bool IsDefaultStyle)>();

			Add("M3MaterialCheckBoxStyle", isImplicit: true);
			Add("M3MaterialAppBarButtonStyle", isImplicit: true);
			Add("M3MaterialCommandBarStyle", isImplicit: true);
			Add("M3MaterialRadioButtonStyle", isImplicit: true);
			Add("M3MaterialDisplayLarge");
			Add("M3MaterialDisplayMedium");
			Add("M3MaterialDisplaySmall");
			Add("M3MaterialHeadlineLarge");
			Add("M3MaterialHeadlineMedium");
			Add("M3MaterialHeadlineSmall");
			Add("M3MaterialTitleLarge");
			Add("M3MaterialTitleMedium");
			Add("M3MaterialTitleSmall");
			Add("M3MaterialLabelLarge");
			Add("M3MaterialLabelMedium");
			Add("M3MaterialLabelSmall");
			Add("M3MaterialBodyLarge");
			Add("M3MaterialBodyMedium", isImplicit: true);
			Add("M3MaterialBodySmall");
			Add("M3MaterialOutlinedTextBoxStyle");
			Add("M3MaterialFilledTextBoxStyle", isImplicit: true);
			Add("M3MaterialOutlinedPasswordBoxStyle");
			Add("M3MaterialFilledPasswordBoxStyle", isImplicit: true);
			Add("M3MaterialElevatedButtonStyle");
			Add("M3MaterialFilledButtonStyle", isImplicit: true);
			Add("M3MaterialFilledTonalButtonStyle");
			Add("M3MaterialOutlinedButtonStyle");
			Add("M3MaterialTextButtonStyle");
			Add("M3MaterialIconButtonStyle");
			Add("M3MaterialCalendarViewStyle", isImplicit: true);
			Add("M3MaterialCalendarDatePickerStyle", isImplicit: true);
			Add("M3MaterialFlyoutPresenterStyle", isImplicit: true);
			Add("M3MaterialMenuFlyoutPresenterStyle", isImplicit: true);
			Add("M3MaterialNavigationViewStyle", isImplicit: true);
			Add("M3MaterialNavigationViewItemStyle", isImplicit: true);
			Add("M3MaterialListViewStyle", isImplicit: true);
			Add("M3MaterialListViewItemStyle", isImplicit: true);
			Add("M3MaterialTextToggleButtonStyle", isImplicit: true);
			Add("M3MaterialIconToggleButtonStyle");
			Add("M3MaterialDatePickerStyle", isImplicit: true);

			return result;

			void Add(string key, string alias = null, bool isImplicit = false) =>
				result.Add((key, alias ?? key.Substring(StylePrefix.Length), isImplicit));
		}
		[Obsolete]
		private static IEnumerable<(string ResourceKey, string SharedKey, bool IsDefaultStyle)> GetToolkitV2StyleInfos()
		{
			const string StylePrefix = "M3Material";
			var result = new List<(string ResourceKey, string SharedKey, bool IsDefaultStyle)>();

			Add("M3MaterialDividerStyle", isImplicit: true);
			Add("M3MaterialNavigationBarStyle", isImplicit: true);
			Add("M3MaterialModalNavigationBarStyle");
			Add("M3MaterialMainCommandStyle", isImplicit: true);
			Add("M3MaterialModalMainCommandStyle");
			Add("M3MaterialTopTabBarStyle");
			Add("M3MaterialColoredTopTabBarStyle");
			Add("M3MaterialElevatedSuggestionChipStyle");
			Add("M3MaterialSuggestionChipStyle");
			Add("M3MaterialInputChipStyle");
			Add("M3MaterialElevatedFilterChipStyle");
			Add("M3MaterialFilterChipStyle");
			Add("M3MaterialElevatedAssistChipStyle");
			Add("M3MaterialAssistChipStyle");
			Add("M3MaterialElevatedSuggestionChipGroupStyle");
			Add("M3MaterialSuggestionChipGroupStyle");
			Add("M3MaterialInputChipGroupStyle");
			Add("M3MaterialElevatedFilterChipGroupStyle");
			Add("M3MaterialFilterChipGroupStyle");
			Add("M3MaterialElevatedAssistChipGroupStyle");
			Add("M3MaterialAssistChipGroupStyle");
			return result;

			void Add(string key, string? alias = null, bool isImplicit = false) =>
				result.Add((key, alias ?? key.Substring(StylePrefix.Length), isImplicit));
		}
		[Obsolete]
		private static IEnumerable<(string ResourceKey, string SharedKey, bool IsDefaultStyle)> ParseGetStyleInfos(string path)
		{
			var source = File.ReadAllText(path);
			var tree = CSharpSyntaxTree.ParseText(source)/*.DumpSyntaxTree()*/;

			var getStyleInfos = tree.GetRoot()
				.DescendantNodes().OfType<MethodDeclarationSyntax>()
				.FirstOrDefault(x => x.Identifier.Text == "GetStyleInfos");

			return getStyleInfos.Body
				//.DumpSyntaxNode()
				.ChildNodes().OfType<ExpressionStatementSyntax>()
				.Select(x => x.Expression)
				.OfType<InvocationExpressionSyntax>()
				.Where(x => (x.Expression as IdentifierNameSyntax)?.Identifier.Text == "Add")
				.Select(x => new
				{
					Key = x.ArgumentList.Arguments[0].Expression.Cast<LiteralExpressionSyntax>().Token.ValueText,
					Implicit = x.ArgumentList.Arguments
						.FirstOrDefault(y => y.NameColon?.Name?.Identifier.ValueText == "isImplicit")
						?.Expression.Cast<LiteralExpressionSyntax>().Token.Value
						as bool? ?? false
				})
				.Select(x => (x.Key, x.Key.RegexReplace("^M3Material", ""), x.Implicit));
		}
	}
	private static partial class Specialized
	{
		public static void CheckLightWeightResourceParity(string path)
		{
			var document = XDocument.Load(path);
			var root = (ResourceDictionary)ScuffedXamlParser.Parse(document.Root);

			var themeResources = root.Values.OfType<ThemeResource>()
				.Select(x => new
				{
					x.Key,
					x.LightValue,
					x.DarkValue,
					Parity = x.AreThemeDefinitionEqual(),
				})
				.ToArray()
				.Dump("ThemeResources", 0);
			var staticResources = root.Values.OfType<StaticResource>()
				.ToArray()
				.Dump("StaticResources", 0);

			Regex.Matches(document.ToString(), @"\{StaticResource (\w+)\}").Cast<Match>()
				.Select(x => x.Groups[1].Value)
				.Where(x => themeResources.Select(x => x.Key).Contains(x))
				.GroupBy(x => x, (k, g) => $"{k} x{g.Count()}")
				.Dump();

			if (themeResources.Count(x => !x.Parity) is { } count && count != 0)
				Util.WithStyle($"{count} of the {themeResources.Length} theme-resources are in disparity", $"color: red").Dump();
			else
				Util.WithStyle($"All {themeResources.Length} theme-resources are in parity", $"color: green").Dump();
		}
		public static string ExtractLightWeightResources(string inspectFile, ResourceDictionary additionalResources)
		{
			var root = (ResourceDictionary)ScuffedXamlParser.Load(inspectFile).Dump(inspectFile, 0);
			var resources = new ResourceDictionary(additionalResources).Merge(root);

			var table = root.Values
				.Where(x => !x.IsStaticResourceFor<Style>())
				.Where(x => x is not StaticResource) // only theme resource should be included for whats considered LightWeight
				.Select(x =>
					x is StaticResource sr ? new { x.Key, RefValue = GetResource(sr.Value) } :
					x is ThemeResource tr ? new { x.Key, RefValue = GetResource(tr.DarkValue) } :
					throw new ArgumentOutOfRangeException()
				)
				.Select(x => new
				{
					x.Key.Key,
					x.RefValue.Type,
					Value = FormatValue(x.RefValue.Value),
				})
				.ToArray()
				.Dump();
			var markdown = table.ToMarkdownTable();
			Clickable.CopyText("Copy as markdown table", markdown).Dump();

			return markdown;

			(string Type, object Value) GetResource(object value)
			{
				if (!(value is StaticResourceRef or ThemeResource))
				{
					return (GetTypename(value), value);
				}

				var innerKey = default(string);
				while (true)
				{
					if (value is StaticResourceRef srr)
					{
						innerKey = srr.Key;
						if (resources[srr.Key] is { } mapped)
						{
							value = mapped;
						}
						else
						{
							return (InferTypenameFromSystemKey(srr.Key), srr.Key);
						}
					}
					else if (value is ThemeResource tr)
					{
						innerKey = tr.Key.Key;
						value = tr.DarkValue;
					}
					else break;
				}

				return (GetTypename(value), innerKey);
			}
			string InferTypenameFromSystemKey(string key)
			{
				var mappings = new (string Pattern, string Replacement)?[]
				{
					("Brush$",  "Brush"),
					("FontFamily$",  "FontFamily"),
					("FontSize$",  "Double"),
					("CornerRadius$",  "CornerRadius"),
				};
				var result = mappings
					.FirstOrDefault(x => Regex.IsMatch(key, x.Value.Pattern))
					?.Replacement;
				if (result != null)
					Util.WithStyle($"Inferring '{key}' as type '{result}'", "color: orange").Dump();

				return result;
			}
			string GetTypename(object value) => value switch
			{
				GenericValueObject gvo => gvo.Typename,
				StaticResource sr => sr.Value?.GetType().Name,

				_ => value?.GetType().Name,
			};
			string FormatValue(object value) => value switch
			{
				GenericValueObject gvo => gvo.Value,

				_ => value?.ToString(),
			};
		}

		public static void DiffThemeV2InnerResources()
		{
			// 26ResKey, 30ResKey, Type, Value: Value or 26Val->30Val
			var sb = new StringBuilder();
			sb.AppendLine(DiffResources(@"X:\src\old\uno.toolkit.ui\src\library\Uno.Toolkit.Material\Styles\Controls\v2", @"X:\src\uno.toolkit.ui\src\library\Uno.Toolkit.Material\Styles\Controls\v2", "Chip.xaml"));
			//sb.AppendLine(DiffResources(@"X:\src\old\uno.toolkit.ui\src\library\Uno.Toolkit.Material\Styles\Controls\v2", @"X:\src\uno.toolkit.ui\src\library\Uno.Toolkit.Material\Styles\Controls\v2", "CardContentControl.xaml"));
			//sb.AppendLine(DiffResources(@"X:\src\old\uno.toolkit.ui\src\library\Uno.Toolkit.Material\Styles\Controls\v2", @"X:\src\uno.toolkit.ui\src\library\Uno.Toolkit.Material\Styles\Controls\v2", "Chip.xaml"));
			//sb.AppendLine(DiffResources(@"X:\src\old\uno.toolkit.ui\src\library\Uno.Toolkit.Material\Styles\Controls\v2", @"X:\src\uno.toolkit.ui\src\library\Uno.Toolkit.Material\Styles\Controls\v2", "ChipGroup.xaml"));
			//sb.AppendLine(DiffResources(@"X:\src\old\uno.toolkit.ui\src\library\Uno.Toolkit.Material\Styles\Controls\v2", @"X:\src\uno.toolkit.ui\src\library\Uno.Toolkit.Material\Styles\Controls\v2", "Divider.xaml"));
			//sb.AppendLine(DiffResources(@"X:\src\old\uno.toolkit.ui\src\library\Uno.Toolkit.Material\Styles\Controls\v2", @"X:\src\uno.toolkit.ui\src\library\Uno.Toolkit.Material\Styles\Controls\v2", "NavigationBar.xaml"));
			//sb.AppendLine(DiffResources(@"X:\src\old\uno.toolkit.ui\src\library\Uno.Toolkit.Material\Styles\Controls\v2", @"X:\src\uno.toolkit.ui\src\library\Uno.Toolkit.Material\Styles\Controls\v2", "TabBar.xaml"));
			//sb.AppendLine(DiffResources(@"X:\src\old\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", @"X:\src\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", "CalendarDatePicker.xaml"));
			//sb.AppendLine(DiffResources(@"X:\src\old\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", @"X:\src\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", "CalendarView.xaml"));
			//sb.AppendLine(DiffResources(@"X:\src\old\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", @"X:\src\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", "CheckBox.xaml"));
			//sb.AppendLine(DiffResources(@"X:\src\old\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", @"X:\src\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", "ComboBox.xaml"));
			//sb.AppendLine(DiffResources(@"X:\src\old\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", @"X:\src\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", "CommandBar.xaml"));
			//sb.AppendLine(DiffResources(@"X:\src\old\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", @"X:\src\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", "ContentDialog.xaml"));
			//sb.AppendLine(DiffResources(@"X:\src\old\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", @"X:\src\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", "DatePicker.xaml"));
			//sb.AppendLine(DiffResources(@"X:\src\old\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", @"X:\src\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", "FloatingActionButton.xaml"));
			//sb.AppendLine(DiffResources(@"X:\src\old\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", @"X:\src\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", "Flyout.xaml"));
			//sb.AppendLine(DiffResources(@"X:\src\old\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", @"X:\src\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", "HyperlinkButton.xaml"));
			//sb.AppendLine(DiffResources(@"X:\src\old\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", @"X:\src\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", "ListView.xaml"));
			//sb.AppendLine(DiffResources(@"X:\src\old\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", @"X:\src\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", "NavigationView.xaml"));
			//sb.AppendLine(DiffResources(@"X:\src\old\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", @"X:\src\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", "PasswordBox.xaml"));
			//sb.AppendLine(DiffResources(@"X:\src\old\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", @"X:\src\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", "PipsPager.xaml", "PipsPager.UWP.xaml", "PipsPager.Base.xaml"));
			//sb.AppendLine(DiffResources(@"X:\src\old\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", @"X:\src\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", "ProgressBar.xaml"));
			//sb.AppendLine(DiffResources(@"X:\src\old\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", @"X:\src\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", "ProgressRing.xaml", "ProgressRingWinUI.xaml"));
			//sb.AppendLine(DiffResources(@"X:\src\old\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", @"X:\src\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", "RadioButton.xaml"));
			//sb.AppendLine(DiffResources(@"X:\src\old\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", @"X:\src\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", "Ripple.xaml"));
			//sb.AppendLine(DiffResources(@"X:\src\old\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", @"X:\src\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", "Slider.xaml"));
			//sb.AppendLine(DiffResources(@"X:\src\old\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", @"X:\src\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", "TextBlock.xaml"));
			//sb.AppendLine(DiffResources(@"X:\src\old\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", @"X:\src\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", "TextBox.xaml"));
			//sb.AppendLine(DiffResources(@"X:\src\old\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", @"X:\src\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", "ToggleButton.xaml"));
			//sb.AppendLine(DiffResources(@"X:\src\old\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", @"X:\src\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2", "ToggleSwitch.xaml"));
			//ExtractInnerResources((ResourceDictionary)ScuffedXamlParser.Load(@"X:\src\old\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2\CalendarDatePicker.xaml")).Dump();
			//ExtractInnerResources(@"X:\src\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2\ProgressRing.xaml").Dump();
			
			Clickable.CopyText("Click to copy result", sb.ToString()).Dump();
			
			(string Key, string Value)[] ExtractInnerResources(ResourceDictionary rd)
			{
				return rd
					.OrderByDescending(x => x.Value is ThemeResource)
					.Where(x => x.Value  is StaticResource sr && sr.Value is not Style)
					.Select(x => ((string Key, string Value))(
						x.Key.ToString(),
						x.Value switch
						{
							StaticResource sr => FormatValue(sr.Value),
							ThemeResource tr => tr.AreThemeDefinitionEqual()
								? FormatValue(tr.LightValue)
								: throw new Exception($"Polarized resource: key={x.Key}"),
							_ => throw new ArgumentOutOfRangeException($"Invalid resource dictionary value type: {x.Value.GetType().Name} (key={x.Key})"),
						}
					))
					.ToArray();
			}
			string DiffResources(string oldBase, string newBase, params string[] files)
			{
				var oldResources = ExtractInnerResources(GetResources(oldBase, files));
				var newResources = ExtractInnerResources(GetResources(newBase, files));
				var keys = oldResources.Concat(newResources).Select(x => x.Key).ToList();
				var sb = new StringBuilder();
				var table = Pair(oldResources, newResources, x => x.Key)
					.OrderByDescending(x => keys.IndexOf(x.Old?.Key ?? x.New?.Key))
					//.Where(x => x.Old != x.New)
					.Select(x => new
					{
						OldKey = x.Old?.Key ?? "- NEWLY ADDED -",
						NewKey = x.New?.Key ?? "**_REMOVED_**",
						Value = CompareValue(x.Old?.Value, x.New?.Value),
					})
					.Dump(Path.GetFileNameWithoutExtension(files.First()))
					.Where(x => x.OldKey != "- NEWLY ADDED -");
					
					

				if (table.Any())
				{
					var md = table.ToMarkdownTable();
					sb.AppendLine($"## {Path.GetFileNameWithoutExtension(files.First())}");
					sb.AppendLine();
					sb.AppendLine(md);
				}
				return sb.ToString();
				//$"# {Path.GetFileNameWithoutExtension(files[0])}".Dump();
				//table.ToMarkdownTable().Dump();
					
				ResourceDictionary GetResources(string basePath, string[] files)
				{
					return files.Select(x => Path.Combine(basePath, x))
						.Where(x => File.Exists(x))
						.Aggregate(new ResourceDictionary(), (acc, x) => acc.Merge((ResourceDictionary)ScuffedXamlParser.Load(x)));
				}
				IEnumerable<(T? Old, T? New)> Pair<T>(IEnumerable<T> oldSource, IEnumerable<T> newSource, Func<T, string> keySelector) where T : struct
				{
					var oldMap = oldSource.ToDictionary(keySelector);
					var newMap = newSource.ToDictionary(keySelector);
					
					foreach (var (o, n) in oldMap.Join(newMap, o => o.Key, n => n.Key, Tuple.Create).ToArray()) // MIDDLE
					{
						oldMap.Remove(o.Key);
						newMap.Remove(n.Key);

						yield return (o.Value, n.Value);
					}
					
					var knownPairs = new List<(string OldKey, string NewKey)>()
					{
						("MaterialComboBoxItemSelectedBackgroundThemeBrush", "ComboBoxItemBackgroundSelected"),
						("MaterialComboBoxArrowForegroundThemeBrush", "ComboBoxArrowForeground"),
						("MaterialComboBoxPlaceholderFocusedThemeBrush", "ComboBoxUpperPlaceHolderForeground"),
						("MaterialComboBoxPlaceholderForegroundThemeBrush", "ComboBoxPlaceHolderForeground"),
						("MaterialDateTimeFlyoutBorderThickness", "DatePickerFlyoutBorderThickness"),
						("MaterialDatePickerFlyoutPresenterBackgroundBrush", "DatePickerFlyoutPresenterBackground"),
						("MaterialDatePickerBackgroundColorBrush", "DatePickerButtonBackground"),
						("_____", "_____"),
						("_____", "_____"),
						("_____", "_____"),
						("_____", "_____"),
						("_____", "_____"),
					};
					foreach (var knownPair in knownPairs) // MIDDLE'
					{
						if (oldMap.TryGetValue(knownPair.OldKey, out var oldValue) && newMap.TryGetValue(knownPair.NewKey, out var newValue))
						{
							oldMap.Remove(knownPair.OldKey);
							newMap.Remove(knownPair.NewKey);

							yield return (oldValue, newValue);
						}
					}
					
					var mutationsT1 = new List<(Func<string, string> OldKeyMutator, Func<string, string> NewKeyMutator)>
					{
						(o => o.Replace("PathData", "Data"), n => n),
						(o => o.RegexReplace("GlyphPathStyle", "GlyphPathData"), n => n),
						(o => Path.GetFileNameWithoutExtension(files[0]) + o, n => n),
						(o => Path.GetFileNameWithoutExtension(files[0]) + o.RegexReplace("^(M3)?Material", ""), n => n),
						(o => o.RegexReplace("BackgroundBrush$", "Brush"), n => n),
						(o => o.RegexReplace("(Theme|Color)Brush$", ""), n => n),
						(o => o.RegexReplace("(Theme|Color)Brush$", "Brush"), n => n),
						(o => o.RegexReplace("(Theme|Color)Brush$", "").RegexReplace("(Selected)?(PointerOver|Pressed|Focused|Unfocused|Disabled)(.+)$", "$3$1$2"), n => n),
						(o => o.RegexReplace("(Theme|Color)Brush$", "").RegexReplace("(Selected)?(PointerOver|Pressed|Focused|Unfocused|Disabled)(.+)$", "$3$1$2"), n => n.Replace("Background", "")),
					};
					var mutationsT2 = new List<(Func<string, string> OldKeyMutator, Func<string, string> NewKeyMutator)>
					{
						(o => o, n => n),
						(o => o.RegexReplace("^(M3)?Material", ""), n => n),
						(o => o.Replace(Path.GetFileNameWithoutExtension(files[0]), ""), n => n.Replace(Path.GetFileNameWithoutExtension(files[0]), "")),
						(o => o.Replace("SurfaceFab", "FabSurface"), n => n),
						(o => o.Replace("SecondaryFab", "FabSecondary"), n => n),
						(o => o.Replace("TertiaryFab", "FabTertiary"), n => n),
					};
					foreach (var t2 in mutationsT2)
					foreach (var t1 in mutationsT1)
					{
						(Func<string, string> OldKeyMutator, Func<string, string> NewKeyMutator) mutation = (
							OldKeyMutator: (string x) => t1.OldKeyMutator(t2.OldKeyMutator(x)), 
							NewKeyMutator: (string x) => t1.NewKeyMutator(t2.NewKeyMutator(x)));
						foreach (var (o, n) in oldMap.Join(newMap, o => mutation.OldKeyMutator(o.Key), n => mutation.NewKeyMutator(n.Key), Tuple.Create).ToArray()) // MIDDLE'
						{
							oldMap.Remove(o.Key);
							newMap.Remove(n.Key);

							yield return (o.Value, n.Value);
						}
					}
					foreach (var o in oldMap) // LEFT
					{
						yield return (o.Value, default);
					}
					// we dont care above new values for migration reference
					//foreach (var n in newMap) // RIGHT
					//{
					//	yield return (default, n.Value);
					//}
				}
			}

			string FormatValue(object value)
			{
				return value switch
				{
					StaticResourceRef srr => srr.Key,
					Style style => $"Style@{style.TargetType}",
					GenericValueObject gvo when gvo.Typename == "LottieVisualSource" => gvo.Value,
					GenericValueObject gvo when gvo.Typename == "GridLength" => gvo.Value,
					GenericValueObject gvo when gvo.Typename == "ControlTemplate" => null,

					_ => value?.ToString(),
				};
			}
			string CompareValue(string o, string n)
			{
				if (o == null) return n;
				if (n == null) return o;
				if (o == n) return o;

				return o.Length + n.Length > 100
					? string.Join("\n", o, "->", n)
					: string.Join(" ", o, "->", n);
			}
		}
	}
}

public record DependencyObject
{
	private Dictionary<string, object> _properties = new();

	public object GetDP(string dp) => _properties.TryGetValue(dp, out var value) ? value : default;
	public void SetDP(string dp, object value) => _properties[dp] = value;
}
public record Thickness(double Left, double Top, double Right, double Bottom)
{
	public override string ToString()
	{
		// format: uniform, [same-left-right,same-top-bottom], [left,top,right,bottom]
		if (Left == Top && Top == Right && Right == Bottom) return $"{Left:0.#}";
		if (Left == Right && Top == Bottom) return $"{Left:0.#},{Top:0.#}";
		return $"{Left:0.#},{Top:0.#},{Right:0.#},{Bottom:0.#}";
	}
}
public record CornerRadius(double TopLeft, double TopRight, double BottomRight, double BottomLeft)
{
	public override string ToString()
	{
		// format: uniform, [left,top,right,bottom]
		if (TopLeft == TopRight && TopRight == BottomRight && BottomRight == BottomLeft) return $"{TopLeft:0.#}";
		return $"{TopLeft:0.#},{TopRight:0.#},{BottomRight:0.#},{BottomLeft:0.#}";
	}
}
public record Color(byte A, byte R, byte G, byte B)
{
	public override string ToString() => "#" + this.ToRgbText();
}
public record SolidColorBrush(Color Color = default, double Opacity = 1) : DependencyObject
{
	public override string ToString()
	{
		var color = GetDP(nameof(Color)) switch
		{
			IResourceRef rf => rf.Key,
			null => Color.ToString(),
			_ => throw new ArgumentOutOfRangeException(),
		};
		var opacity = GetDP(nameof(Opacity)) switch
		{
			IResourceRef rf => $"*{rf.Key}",
			null when Opacity != 1 => $"*{Opacity}",
			null => "",
			_ => throw new ArgumentOutOfRangeException(),
		};

		return $"{color}{opacity}";
	}
}
public record Style(string Key = null, string TargetType = null, string BasedOn = null)
{
	public List<Setter> Setters = new();

	private object ToDump() => new { Key, TargetType, BasedOn, Setters };

	public static Style ParseStyle(XElement e)
	{
		var result = new Style(
			ResourceDictionary.GetKey(e),
			e.Attribute("TargetType")?.Value,
			IKeyedResource.GetKeyFromMarkup(e.Attribute("BasedOn")?.Value)
		);
		foreach (var child in e.Elements())
		{
			if (child.Name == (Presentation + "Setter"))
			{
				ParseChild(child);
			}
			if (child.Name == (Presentation + "Styles.Setter"))
			{
				foreach (var setter in child.Elements())
				{
					ParseChild(child);
				}
			}
		}

		void ParseChild(XElement child)
		{
			if (child.Name == (Presentation + "Setter"))
			{
				result.Setters.Add(ScuffedXamlParser.ParseSetter(child));
			}
			else
			{
				throw new NotImplementedException("Unknown member: " + child.Name);
			}
		}

		return result;
	}
}
public record Setter(string Property, string Value);

public record GenericValueObject(string Typename, string Value);

public record ResourceKey(string Key, string TargetType = null) : IComparable
{
	public static implicit operator ResourceKey(string key) => new(key);

	private object ToDump() => ToString();
	public override string ToString() => Key?.ToString() ?? $"TargetType={TargetType}";

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
}
public interface IKeyedResource
{
	ResourceKey Key { get; }

	public static string GetKeyFromMarkup(string markup)
	{
		if (markup is null) return null;

		return TryGetKeyFromMarkup(markup, out var key)
			? key
			: throw new ArgumentException("Invalid resource markup: " + markup);
	}
	public static bool TryGetKeyFromMarkup(string maybeMarkup, out string key)
	{
		if (maybeMarkup is not null && Regex.Match(maybeMarkup, @"^{(?<type>(Static|Theme|Dynamic)Resource) (?<key>\w+)}$") is { Success: true } match)
		{
			key = match.Groups["key"].Value;
			return true;
		}
		else
		{
			key = default;
			return false;
		}
	}
	public static IResourceRef TryParseResource(string maybeMarkup)
	{
		if (maybeMarkup is not null && Regex.Match(maybeMarkup, @"^{(?<type>(Static|Theme|Dynamic)Resource) (?<key>\w+)}$") is { Success: true, Groups: var g })
		{
			return g["type"].Value switch
			{
				"StaticResource" => new StaticResourceRef(g["key"].Value),
				"ThemeResource" => new ThemeResourceRef(g["key"].Value),
				"DynamicResource" => throw new NotImplementedException("DynamicResource"),
				
				_ => throw new ArgumentOutOfRangeException($"Invalid resource markup: {g["type"].Value}"),
			};
		}
		
		return default;
	}
}
public interface IResourceRef
{
	string Key { get; }
}
public record StaticResource(ResourceKey Key, object Value) : IKeyedResource;
public record ThemeResource(ResourceKey Key, object LightValue, object DarkValue) : IKeyedResource // naive impl only, it should be a dict<TKey, object>
{
	public bool AreThemeDefinitionEqual()
	{
		return LightValue.Equals(DarkValue);
	}
}
public record StaticResourceRef(string Key) : IResourceRef;
public record ThemeResourceRef(string Key) : IResourceRef;

public static class Global
{
	public static readonly XNamespace Presentation = XNamespace.Get(NSPresentation);
	public static readonly XNamespace X = XNamespace.Get(NSX);

	public const string NSX = "http://schemas.microsoft.com/winfx/2006/xaml";
	public const string NSPresentation = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
}
public class ResourceDictionary : Dictionary<ResourceKey, IKeyedResource>
{
	public static Dictionary<string, string> ThemeMapping = new()
	{
		["Light"] = "Light",
		["Dark"] = "Dark",
	};

	public ResourceDictionary() { }
	public ResourceDictionary(ResourceDictionary rd) : base(rd) { }

	public void Add(IKeyedResource resource)
	{
		if (!TryAdd(resource.Key, resource))
		{
#if ALLOW_DUPLICATED_KEYS
#if !ALLOW_DUPLICATED_KEYS_WITHOUT_WARNING
			Util.WithStyle($"Duplicated key: {resource.Key}", "color: orange").Dump();
#endif
#else
			throw new ArgumentException($"Duplicated key: {resource.Key}");
#endif
		}
	}
	public void AddRange(IEnumerable<IKeyedResource> resources)
	{
		foreach (var resource in resources)
			Add(resource);
	}
	public ResourceDictionary Merge(ResourceDictionary other)
	{
		AddRange(other.Values);
		return this;
	}

	public object ToDump() => Values;

	public IKeyedResource this[string Key = null, string TargetType = null]
	{
		get => this.TryGetValue(new(Key, TargetType), out var value) ? value : default;
	}

	public static string GetKey(XElement element)
	{
		// https://docs.microsoft.com/en-us/windows/apps/design/style/xaml-resource-dictionary
		// x:Name can be used instead of x:Key. However, [...less optimal].
		return
			element.Attribute(X + "Key")?.Value ??
			element.Attribute(X + "Name")?.Value;
	}
	private static ResourceKey GetResourceKey(XElement element)
	{
		if (GetKey(element) is string key)
		{
			return key;
		}
		if (element.Attribute("TargetType")?.Value is { } targetType)
		{
			return new(null, targetType);
		}

		element.Dump();
		throw new KeyNotFoundException();
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
					"ThemeDictionaries" => ParseThemeDictionaries(result, child),
					"MergedDictionaries" => ParseMergedDictionaries(result, child),

					_ => throw new NotImplementedException(property),
				};
			}
			else
			{
				result.Add(new StaticResource(ResourceDictionary.GetResourceKey(child), ScuffedXamlParser.Parse(child)));
			}
		}

		return result;
	}
	protected static ResourceDictionary[] ParseThemeDictionaries(ResourceDictionary rd, XElement e)
	{
		var themes = Parse(e);
		var light = (ResourceDictionary)((StaticResource)themes.FirstOrDefault(x => ThemeMapping["Dark"].Split(',').Contains(x.Key.Key)).Value).Value;
		var dark = (ResourceDictionary)((StaticResource)themes.FirstOrDefault(x => ThemeMapping["Dark"].Split(',').Contains(x.Key.Key)).Value).Value;

		object GetResourceUnwrapped(ResourceDictionary rd, ResourceKey key) => ((StaticResource)rd[key]).Value;
		foreach (var key in Enumerable.Union(light.Keys, dark.Keys))
			rd.Add(new ThemeResource(key, GetResourceUnwrapped(light, key), GetResourceUnwrapped(dark, key)));

		return new[] { light, dark };
	}
	protected static ResourceDictionary[] ParseMergedDictionaries(ResourceDictionary rd, XElement e)
	{
		var buffer = new List<ResourceDictionary>();
		foreach (var item in e.Elements())
		{
			var nested = Parse(item);
			rd.Merge(nested);
		}

		return buffer.ToArray();
	}
}
public static class ScuffedXamlParser
{
	public static object Load(string path)
	{
		Util.Metatext($"=== parsing: {path}").Dump();
		var document = XDocument.Load(path);
		return ScuffedXamlParser.Parse(document.Root);
	}
	public static object Parse(XElement e)
	{
		return e.GetNameParts() switch
		{
			(NSX, nameof(Boolean)) => bool.Parse(e.Value),
			(NSX, nameof(Int32)) => int.Parse(e.Value),
			(NSX, nameof(Double)) => double.Parse(e.Value),
			(NSX, nameof(String)) => e.Value,
			(NSPresentation, nameof(ResourceDictionary)) => ResourceDictionary.Parse(e),
			(NSPresentation, nameof(Thickness)) => ParseThickness(e.Value),
			(NSPresentation, nameof(CornerRadius)) => ParseCornerRadius(e.Value),
			(NSPresentation, nameof(Color)) => ParseColor(e.Value),
			(NSPresentation, nameof(SolidColorBrush)) => ParseSolidColorBrush(e),
			(NSPresentation, nameof(StaticResource)) => ParseStaticResource(e),
			(_/*NSPresentation*/, nameof(Style)) => Style.ParseStyle(e),
#if ENABLE_GENERIC_VALUE_OBJECT_PARSING
			(NSPresentation, "FontFamily") => new GenericValueObject("FontFamily", e.Value),
			(NSPresentation, "GridLength") => new GenericValueObject("GridLength", e.Value),
			(NSPresentation, "ControlTemplate") => new GenericValueObject("ControlTemplate", "parser-not-implemented"),
			(_, "LottieVisualSource") => new GenericValueObject("LottieVisualSource", e.Attribute("UriSource").Value),
#endif

			_ when e.Name.LocalName.EndsWith("Converter") => null,
#if !THROW_ON_NOTIMPLEMENTED_OBJECT
			_ => LogNotImplementedMessage(e.Name.LocalName),
#else
			_ => throw new NotImplementedException(e.Name.ToString()),
#endif
		};

		object LogNotImplementedMessage(string type)
		{
			Util.Metatext($"Ignoring '{type}', as there is no parser implemented for it").Dump();
			return null;
		}
	}

	public static Thickness ParseThickness(string raw)
	{
		try
		{
			return raw.Split(',') switch
			{
				[var uniform] when double.TryParse(uniform, out var u) => new(u, u, u, u),
				[var lr, var tb] when
					double.TryParse(lr, out var lrv) &&
					double.TryParse(tb, out var tbv)
					=> new(lrv, tbv, lrv, tbv),
				[var l, var t, var r, var b] when
					double.TryParse(l, out var lv) &&
					double.TryParse(t, out var tv) &&
					double.TryParse(r, out var rv) &&
					double.TryParse(b, out var bv)
					=> new(lv, tv, rv, bv),

				_ => throw new FormatException("Invalid corner-radius syntax"),
			};
		}
		catch (Exception ex)
		{
			throw new ArgumentException("Invalid corner-radius literal: " + raw, ex);
		}
	}
	public static CornerRadius ParseCornerRadius(string raw)
	{
		try
		{
			return raw.Split(',') switch
			{
				[var uniform] when double.TryParse(uniform, out var u) => new(u, u, u, u),
				[var tl, var tr, var br, var bl] when
					double.TryParse(tl, out var tlv) &&
					double.TryParse(tr, out var trv) &&
					double.TryParse(br, out var brv) &&
					double.TryParse(bl, out var blv)
					=> new(tlv, trv, brv, blv),

				_ => throw new FormatException("Invalid corner-radius syntax"),
			};
		}
		catch (Exception ex)
		{
			throw new ArgumentException("Invalid corner-radius literal: " + raw, ex);
		}
	}
	public static Color ParseColor(string raw)
	{
		// #rgb (need to check definition...), #rrggbb, #aarrggbb
		if (raw.StartsWith("#") && raw[1..] is { Length: /*3 or*/ 6 or 8 } hex && Regex.IsMatch(hex, "^[a-f0-9]+$", RegexOptions.IgnoreCase))
		{
			var parts = Enumerable.Range(0, hex.Length / 2).Select(x => byte.Parse(hex.Substring(x * 2, 2), NumberStyles.HexNumber)).ToArray();
			return parts.Length switch
			{
				3 => new Color(byte.MaxValue, parts[0], parts[1], parts[2]),
				4 => new Color(parts[0], parts[1], parts[2], parts[3]),

				_ => throw new ArgumentException("Invalid color literal: " + raw),
			};
		}

		return raw switch
		{
			"Transparent" => new Color(0, 255, 255, 255),

			_ => throw new ArgumentException("Invalid color literal: " + raw),
		};
	}
	public static SolidColorBrush ParseSolidColorBrush(XElement e)
	{
		var result = new SolidColorBrush();
		if (result.MapDP(e, x => x.Color, out var color)) result = result with { Color = ParseColor(color) };
		if (result.MapDP(e, x => x.Opacity, out var opacity)) result = result with { Opacity = double.Parse(opacity) };

		return result;
	}
	public static Setter ParseSetter(XElement e)
	{
		return new(e.Attribute("Property")?.Value, e.Attribute("Value")?.Value);
	}
	private static object ParseStaticResource(XElement e)
	{
		return new StaticResourceRef(e.Attribute("ResourceKey")?.Value);
	}
	
	private static bool MapDP<T,TProperty>(this T obj, XElement e, Expression<Func<T, TProperty>> memberSelector, out string value) where T : DependencyObject
	{
		var property = memberSelector.Body switch
		{
			MemberExpression m => m.Member.Name,
		
			_ => throw new ArgumentOutOfRangeException(memberSelector.Body.Type.ToString()),
		};
		if (e.Attribute(property) is { } attribute)
		{
			if (IKeyedResource.TryParseResource(attribute.Value) is { } resRef)
			{
				obj.SetDP(property, resRef);
			}
			else
			{
				value = attribute.Value;
				return true;
			}
		}
		
		value = default;
		return false;
	}
}

public static class EnumerableExtensions
{
	public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
	{
		foreach (var item in source)
		{
			action(item);
		}
	}
	public static string JoinBy<T>(this IEnumerable<T> source, string separator)
	{
		return string.Join(separator, source);
	}
}
public static class StringExtensions
{
	public static string RegexReplace(this string input, string pattern, string replacement) => Regex.Replace(input, pattern, replacement);
}
public static class XElementExtensions
{
	public static Dictionary<string, string> GetStyleSetters(this XElement element)
	{
		return element
			.Descendants(Presentation + "Setter")
			.ToDictionary(x => x.Attribute("Property").Value, x => x.Attribute("Value").Value);
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
public static class XAttributeExtensions
{
	public static string ResourceKey(this XAttribute attribute)
	{
		var match = Regex.Match(attribute.Value, @"^{(StaticResource|ThemeResource) (?<key>.+)}$");

		return match.Success ? match.Groups["key"].Value : null;
	}
}
public static class ColorHelper
{
	public static string ToRgbText(this Color color) => $"{color.R:X2}{color.G:X2}{color.B:X2}";
	public static string ToRgbText(this SolidColorBrush brush) => brush.Color.ToRgbText();

	public static object ToColoredBlock(this Color color)
	{
		var background = $"#{color.R:X2}{color.G:X2}{color.B:X2}";
		return Util.RawHtml($"<div style='height: 20px; width: 20px; background: #{color.ToRgbText()};' />");
	}
	public static object ToColoredBlock(this SolidColorBrush brush) => brush.Color.ToColoredBlock();

	public static string ToPrettyString(this Color color)
	{
		return $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2} (rgb: {color.R} {color.G} {color.B}, alpha: {color.A})";
	}
	public static string ToPrettyString(this SolidColorBrush brush) => brush.Color.ToPrettyString();
}
public static class MarkdownHelper
{
	public static string ToMarkdownBlock(this Color color) => $"![](http://via.placeholder.com/50x25/{color.ToRgbText()}/{color.ToRgbText()})";
	public static string ToMarkdownBlock(this SolidColorBrush brush) => brush.Color.ToMarkdownBlock();

	public static string ToMarkdownTable<T>(this IEnumerable<T> source)
	{
		var properties = typeof(T).GetProperties();
		var buffer = new StringBuilder();

		// header
		buffer
			.AppendLine(string.Join("|", properties.Select(x => x.Name)))
			.AppendLine(string.Join("|", Enumerable.Repeat("-", properties.Length)));

		// content
		foreach (var item in source)
		{
			buffer.AppendLine(string.Join("|", properties
				.Select(p => p.GetValue(item))
			));
		}

		return buffer.ToString();
	}
	public static Hyperlinq ToCopyableMarkdownTable<T>(this IEnumerable<T> source, string header = "Copy as md table") => Clickable.CopyText(header, source.ToMarkdownTable());
}
public static class KeyedResourceExtensions
{
	public static bool IsReferenceFor<T>(this IKeyedResource x)
	{
		return x switch
		{
			StaticResource sr => sr.Value is T,
			ThemeResource tr => tr.LightValue is T && tr.DarkValue is T,

			_ => false,
		};
	}
	public static bool IsStaticResourceFor<T>(this IKeyedResource x) => (x as StaticResource)?.Value is T;
	public static bool IsThemeResourceFor<T>(this IKeyedResource x) => x is ThemeResource tr && tr.LightValue is T && tr.DarkValue is T;

	public static IEnumerable<T> OfStaticResourceType<T>(this IEnumerable<IKeyedResource> source)
	{
		return source
			.Select(x => (x as StaticResource)?.Value)
			.OfType<T>();
	}
}
public static class FluentExtensions
{
	public static TResult Apply<T, TResult>(this T value, Func<T, TResult> selector)
	{
		return value != null ? selector(value) : default;
	}
}
public static class PivotHelper
{
	public static IEnumerable<ExpandoObject> Pivot<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector, Func<T, IDictionary<string, object>> columnSelectors)
	{
		var results = new Dictionary<TKey, ExpandoObject>();
		foreach (var item in source)
		{
			var key = keySelector(item);
			if (!results.TryGetValue(key, out var data))
			{
				results[key] = data = new ExpandoObject();
				(data as dynamic).Key = key;
			}

			var columns = columnSelectors(item);
			columns.Populate(data);
		}

		return results.Values;
	}

	private static ExpandoObject Populate(this IDictionary<string, object> source, ExpandoObject target)
	{
		var targetImpl = target as IDictionary<string, object>;
		foreach (var kvp in source)
		{
			targetImpl[kvp.Key] = kvp.Value;
		}

		return target;
	}
}
public static class CodeAnalysisExtensions
{
	public static T As<T>(this SyntaxNode x) where T : SyntaxNode => x as T;
	public static T Cast<T>(this SyntaxNode x) where T : SyntaxNode => (T)x;
}
public static class Clickable
{
	public static Hyperlinq CopyText(string text) => CopyText(text, text);
	public static Hyperlinq CopyText(string header, string text)
	{
		return Create(header, () => ClipboardService.SetText(text));
	}

	public static Hyperlinq Create(string header, Action action) => new Hyperlinq(action, header);
}