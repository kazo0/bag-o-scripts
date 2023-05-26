<Query Kind="Program" />

// tags: xaml, crawler
// description: list the resources defined and their usage statistics in the specified *.xaml file.
// setup: first line

public partial class Script
{
	public static void Main()
	{
		Util.AutoScrollResults = false;
		
		//Test();
		//CrawlResourceDependencies(@"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\Styles\Controls\NavigationView\WUX");
		//CrawlResourceDependencies(@"D:\code\uno\platform\Uno.Themes\src\samples\Uno.Themes.Samples\Uno.Themes.Samples.Shared\Styles\", @"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\Styles\Controls\NavigationView\NavigationView_MUX.xaml", sort: true);
		
		// note: no need to add the same XamlMergeInput[Remove] here for colors or fonts.
		// we still need these explicit references here, since the parser wont handle merged dictionary import
		var m1Context = new ProjectResourceContextProvider(@"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\")
			.Include(@"Styles\Application\v1\**\*.xaml")
			.Include(@"Styles\Controls\v1\**\*.xaml")
			.Include(@"Styles\Application\Common\**\*.xaml");
		var m2Context = new ProjectResourceContextProvider(@"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\")
			.Include(@"Styles\Application\v2\**\*.xaml")
			.Include(@"Styles\Controls\v2\**\*.xaml")
			.Include(@"Styles\Application\Common\**\*.xaml")
			.WithAliases(ParseMaterialV2Aliases());
		var figmaContext = new ProjectResourceContextProvider(@"D:\code\uno\platform\Uno.Figma\src\Uno.Figma.Plugin.Editor.Shared\")
			.Include(@"Styles\Application\*.xaml")
			.Include(@"Styles\Controls\*.xaml");
		var emptyContext = new ProjectResourceContextProvider(@"D:\code\uno\");
		//todo: new MultiProjectResourceContextProvider().Local().Foreign()
		
		CrawlResourceDependencies(m2Context, @"D:\code\uno\platform\Uno.Figma\src\Uno.Figma.Plugin.Editor.Shared\Styles\Controls\Button.xaml", sort: true);
		
		//CrawlResourceDependencies(m2Context, @"D:\code\uno\platform\Uno.Themes\src/library/Uno.Material/Styles/Controls/v2/CommandBar.xaml", sort: true );
		//for (int i = 0; i < 3; i++) Console.WriteLine("================================================================================");
		//CrawlResourceDependencies(m1Context, @"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\Styles\Controls\v1\CalendarView.xaml", sort: true );
		//CrawlResourceDependencies(m2Context, @"D:\code\uno\platform\Uno.UI.Toolkit\src\library\Uno.Toolkit.Material\Styles\Controls\v2\Chip.xaml", sort: true );
		//for (int i = 0; i < 3; i++) Console.WriteLine("================================================================================");
		//CrawlResourceDependencies(m1Context, @"D:\code\uno\platform\Uno.UI.Toolkit\src\library\Uno.Toolkit.Material\Styles\Controls\v1\Chip.xaml", sort: true );
		
		// Figma
		// Material(?=\w+(Color|Brush))
		// Material(?=\w*(ToggleSwitchStyle))
	}

	private static Dictionary<string, string> ParseMaterialV2Aliases()
	{
		var source = File.ReadAllText(@"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\MaterialResourcesV2.cs");
		var keys = Regex.Matches(source, @"(?<=Add\("")M3\w+(?="")")
			.Select(x => x.Value)
			.ToArray();
		// note: in themes, it is done with `x.Substring("M3Material".Length)`... lets not do that
		var aliases = keys.ToDictionary(x => Regex.Replace(x, "^M3Material", ""));
		
		return aliases;
	}
	private static void CrawlResourceDependencies(ProjectResourceContextProvider context)
	{
		var xamls = context.GetResources()
			.Select(x => new { Source = PathHelper.MakeRelative(x, context.BasePath), Content = File.ReadAllText(x) })
			.ToArray();
		
		var definitions = xamls
			.SelectMany(x => GetResourceDefinitions(x.Content).Select(y => new { Key = y, x.Source }))
			.ToArray();
		var references = xamls
			.SelectMany(x => GetResourceReferences(x.Content).Select(y => new { Key = y, x.Source }))
			.ToArray();

		definitions
			.Select(x => new
			{ 
				x.Key, 
				ReferenceCount = Util.HighlightIf(references.Count(y => y.Key == x.Key), y => y == 0),
				x.Source
			})
			.Dump("Definitions", 0);
		references
			.GroupBy(x => new { x.Key, x.Source })
			.Select(g => new
			{
				g.Key.Key, g.Key.Source,
				Count = g.Count(),
				From = 
					definitions.Any(x => x.Key == g.Key.Key && x.Source == g.Key.Source) ? "local" :
					definitions.Any(x => x.Key == g.Key.Key) ? Util.Highlight("global", "orange") : 
					Util.Highlight("external")
			})
			.OrderBy(x => x.From.Equals("local"))
			.ThenBy(x => x.Source)
			.ThenBy(x => x.Key)
			.Dump("References");
	}
	private static void CrawlResourceDependencies(ProjectResourceContextProvider context, string focusIntoFile, bool sort = true)
	{
		var xamls = context.GetResources()
			.Append(focusIntoFile).Distinct()
			.Select(x => new { Source = PathHelper.MakeRelative(x, context.BasePath), Content = File.ReadAllText(x), IsFocused = x == focusIntoFile })
			.ToArray();
		
		var definitions = xamls
			.SelectMany(x => GetResourceDefinitions(x.Content).Select(y => new { Key = y, x.Source, x.IsFocused }))
			.ToArray();
		var references = xamls
			.SelectMany(x => GetResourceReferences(x.Content).Select(y => new { Key = y, x.Source, x.IsFocused }))
			.ToArray();

		definitions
			.Where(x => x.IsFocused)
			.Select(x => new
			{ 
				x.Key, 
				ReferenceCount = Util.HighlightIf(references.Count(y => y.Key == x.Key), y => y == 0),
				x.Source
			})
			.Dump("Definitions", 0);
		var temp0 = references
			.Where(x => x.IsFocused)
			.GroupBy(x => new { x.Key, x.Source })
			.Select(g => new
			{
				g.Key.Key,
				UsedIn = g.Key.Source,
				Count = g.Count(),
				Origin = definitions.FirstOrDefault(x => x.Key == g.Key.Key)?.Source,
				OriginType =
					definitions.Any(x => x.Key == g.Key.Key && x.Source == g.Key.Source) ? ResourceOrigin.Local :
					definitions.Any(x => x.Key == g.Key.Key) ? ResourceOrigin.Project : 
					ResourceOrigin.Foreign
			})
			.ApplyIf(sort, source => source
				.OrderByDescending(x => x.OriginType)
				.ThenBy(x => x.UsedIn)
				.ThenBy(x => x.Key)
			)
			.ToArray();

		Util.VerticalRun(
			string.Join(", ", temp0.OrderBy(x => x.OriginType).GroupBy(x => x.OriginType, (k, g) => $"{k}: {g.Count()}")),
			temp0
				.Select(x => new
				{
					x.Key,
					x.Count,
					OriginType = Util.WithStyle(x.OriginType, x.OriginType switch
					{
						ResourceOrigin.Local => "",
						ResourceOrigin.Project => "background: green",
						ResourceOrigin.Foreign => "background: yellow; color: red",
					}),
					Origin = x.Origin,
					x.UsedIn,
				})
		).Dump("References");
		
	}
}
public partial class Script
{
	public static IEnumerable<string> FindXamlFiles(string path, string folderIgnore = null)
	{
		return DirectoryHelper
			.EnumerateAllNestedDirectories(path, x => (folderIgnore ?? ".git,.vs,bin,obj").Split(',').All(y => !x.EndsWith(y)))
			.Prepend(path)
			.SelectMany(x => Directory.GetFiles(x, "*.xaml"));
	}
	public static IEnumerable<string> GetResourceDefinitions(string xaml)
	{
		return Regex.Matches(xaml, @"x:Key=""(?<key>[^""]+)""")
			.Cast<Match>()
			.Select(x => x.Groups["key"].Value);
	}
	public static IEnumerable<string> GetResourceReferences(string xaml)
	{
		return Regex.Matches(xaml, @"{(Static|Theme)Resource (?<key>[^}]+)}")
			.Concat(Regex.Matches(xaml, @"ResourceKey=""(?<key>\w+)"""))
			.Cast<Match>()
			.Select(x => x.Groups["key"].Value);
	}
}

// You can define other methods, fields, classes and namespaces here
public enum ResourceOrigin
{
	Local, Project, Foreign, Unknown
}
public class ProjectResourceContextProvider
{
	public string BasePath { get; init; }
	public List<string> Includes { get; } = new();
	public List<string> Excludes { get; } = new();
	public Dictionary<string, string> Aliases { get; } = new();
	
	public ProjectResourceContextProvider(string basePath)
	{
		this.BasePath = basePath;
	}
	public static implicit operator ProjectResourceContextProvider(string basePath)
	{
		return new ProjectResourceContextProvider(basePath).Include("*");
	}
	
	public ProjectResourceContextProvider Include(string pattern)
	{
		Includes.Add(pattern);
		return this;
	}
	public ProjectResourceContextProvider Exclude(string pattern)
	{
		Excludes.Add(pattern);
		return this;
	}
	public ProjectResourceContextProvider WithAliases(Dictionary<string, string> aliases)
	{
		return this;
	}
	
	public IEnumerable<string> GetResources()
	{
		var included = MatchPattern(Includes);
		var excluded = MatchPattern(Excludes);
		var result = included.Except(excluded).ToArray();
		
		return result.OrderBy(x => x);
		
		bool NotGenerated(string directoryPath) =>  ".git,.vs,bin,obj".Split(',').All(x => !directoryPath.EndsWith(x));
		IEnumerable<string> MatchPattern(List<string> patterns)
		{
			var list = new List<string>();
			foreach (var item in patterns)
			{
				var pattern = PathHelper.UnifyDirectorySeparator(Path.Combine(BasePath, item));
				if (pattern.Contains("*")) // handle pre-defined wildcard cases
				{
					// just wildcard filename: * OR *.ext
					if (Regex.Match(pattern, @"^(?<path>[^*]+\\)(?<pattern>\*(\.\w+)?)$") is Match { Success: true } m0)
					{
						list.AddRange(Directory.GetFiles(m0.Groups["path"].Value, m0.GetCapture("pattern") ?? "*"));
					}
					// wildcard path and/or no/normal/wildcard filename: **\ OR **\* OR **\fixed.name
					else if (Regex.Match(pattern, @"^(?<path>[^*]+\\)\*\*\\(?<pattern>(\*|\w+)(\.\w+)?)?$") is Match { Success: true } m1)
					{
						list.AddRange(DirectoryHelper
							.EnumerateAllNestedDirectories(m1.Groups["path"].Value, NotGenerated)
							.Prepend(m1.Groups["path"].Value)
							.SelectMany(x => Directory.GetFiles(x, m1.GetCapture("pattern") ?? "*"))
						);
					}
					else
					{
						throw new ArgumentException($"Invalid pattern: {pattern}");
					}
				}
				else if (File.Exists(pattern))
				{
					list.Add(pattern);
				}
				else if (Directory.Exists(pattern))
				{
					list.AddRange(DirectoryHelper
						.EnumerateAllNestedDirectories(pattern, NotGenerated)
						.Prepend(pattern)
						.SelectMany(x => Directory.GetFiles(x))
					);
				}
			}
			
			return list;
		}
	}
}

public static class FluentExtensions
{
	public static TResult Apply<T, TResult>(this T target, Func<T, TResult> selector) => selector(target);
	public static T ApplyIf<T>(this T target, bool predicate, Func<T, T> selector) => predicate ? selector(target) : target;
}
public static class DirectoryHelper
{
	public static IEnumerable<string> EnumerateAllNestedDirectories(string path) => EnumerateAllNestedDirectories(path, _ => true);
	public static IEnumerable<string> EnumerateAllNestedDirectories(string path, Func<string, bool> predicate)
	{
		if (!Directory.Exists(path)) yield break;
		foreach (var directory in Directory.EnumerateDirectories(path))
		{
			if (predicate(directory))
			{
				yield return directory;
				foreach (var subdirectory in EnumerateAllNestedDirectories(directory, predicate))
				{
					yield return subdirectory;
				}
			}
		}
	}
}
public static class PathHelper
{
	public static string MakeRelative(string path, string relativeTo)
	{
		relativeTo = relativeTo.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
		
		return new Uri(relativeTo).MakeRelativeUri(new Uri(path)).OriginalString;
	}
	
	public static string UnifyDirectorySeparator(string path)
	{
		return path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
	}
}
public static class RegexHelper
{
	public static string GetCapture(this Match m, string name)
	{
		return m.Groups[name] is { Success: true} g ? g.Value : default;
	}
}