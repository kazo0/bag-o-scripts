<Query Kind="Program" />

#nullable enable

public partial class Script
{
	public static void Main() => 
		//Test();
		Crawl();

	static void Crawl()
	{
		Util.AutoScrollResults = false;
		
		// note: no need to add the same XamlMergeInput[Remove] here for colors or fonts.
		// we still need these explicit references here, since the parser wont handle merged dictionary import
		#region context providers
		var uwpContext = new ProjectResourceContextProvider(@"D:\code\uno\framework\Uno\src\")
			.Include(@"Uno.UI\UI\Xaml\Style\mergedstyles.xaml")
			.Include(@"Uno.UI\UI\Xaml\Style\Generic\SystemResources.xaml")
			.Include(@"Uno.UI.FluentTheme.v1\themeresources_v1.xaml")
			.Include(@"Uno.UI.FluentTheme.v2\themeresources_v2.xaml");
		var m1Context = new ProjectResourceContextProvider(@"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\")
			.Include(@"Styles\Application\v1\**\*.xaml")
			.Include(@"Styles\Controls\v1\**\*.xaml")
			.Include(@"Styles\Application\Common\**\*.xaml");
		var m2Context = new ProjectResourceContextProvider(@"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\")
			.Include(@"Styles\Application\v2\**\*.xaml")
			.Include(@"Styles\Controls\v2\**\*.xaml")
			.Include(@"Styles\Application\Common\**\*.xaml")
			.WithAliases(ParseMaterialV2Aliases(@"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\MaterialResourcesV2.cs"));
		var	iContext = new ProjectResourceContextProvider(@"D:\code\uno\platform\Uno.Themes\src\library\Uno.Cupertino\")
			.Include(@"Styles\Application\*.xaml")
			.Include(@"Styles\Controls\*.xaml");
		var figma = new ProjectResourceContextProvider(@"D:\code\uno\platform\Uno.Figma\src\Uno.Figma.Plugin.Editor.Shared\")
			.Include(@"Styles\Application\*.xaml")
			.Include(@"Styles\Controls\*.xaml");
		var gallery = new ProjectResourceContextProvider(@"D:\code\uno\platform\Uno.Gallery\Uno.Gallery\Uno.Gallery.UWP\Views\")
			.Include(@"Styles\*.xaml")
			//.Include(@"*.xaml");
			.Include(@"Colors.xaml")
			.Include(@"Converters.xaml");
		var emptyContext = new ProjectResourceContextProvider(@"D:\code\platform\Uno.Todo");
		#endregion
		
		var focus = @"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2\CheckBox.xaml";
		//var focus = @"D:\code\uno\platform\Uno.Gallery\Uno.Gallery\Uno.Gallery.UWP\Views\Styles\ColorPaletteView.xaml";
		var context = new ProjectResourceContextProvider(focus?.Apply(FindGitRootFolder) ?? @"D:\code\uno\tests\dontcare")
			/* ===== just declare them in the same order as app.xaml; CrawlResourceDependencies check for LastOrDefault =====*/
			//.Include(@"Styles\Application\*.xaml")
			//.Include(@"Styles\Controls\*.xaml")
			.Include(SourceRelation.Foreign, "uwp/uno", uwpContext)
			//.Include(SourceRelation.Unknown, "theme v1", m1Context)
			.Include(SourceRelation.Project, "theme v2", m2Context)
			//.Include(SourceRelation.Foreign, "theme ios", iContext)
			.WithAliases(ParseMaterialV2Aliases(@"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\MaterialResourcesV2.cs"))
			#if false // toolkit
			//.Include(SourceRelation.Foreign, "toolkit", @"D:\code\uno\platform\Uno.Toolkit\src\Uno.Toolkit.UI\Controls\**\*.xaml")
			//.Include(SourceRelation.Foreign, "toolkit-ios", @"D:\code\uno\platform\Uno.Toolkit\src\library\Uno.Toolkit.Cupertino\Styles\Controls\*.xaml")
			//.Include(SourceRelation.Unknown, "toolkit-mat v1", @"D:\code\uno\platform\Uno.UI.Toolkit\src\library\Uno.Toolkit.Material\Styles\Controls\v1\**\*.xaml")
			//.Include(SourceRelation.Foreign, "toolkit-mat v2", @"D:\code\uno\platform\Uno.UI.Toolkit\src\library\Uno.Toolkit.Material\Styles\Controls\v2\**\*.xaml")
			.WithAliases(ParseMaterialV2Aliases(@"D:\code\uno\platform\Uno.Toolkit\src\library\Uno.Toolkit.Material\MaterialToolkitResourcesV2.cs"))
			#endif
			//.Include(SourceRelation.Local, "test", @"D:\code\uno\tests\NVLab\NVLab.Shared\SV.xaml")
			;
		
		//autoContext.GetResources().Dump();
		CrawlResourceDependencies(context, focus, sort: true);
	}
	static void Test()
	{
		var path = @"D:\code\uno\platform\Uno.Themes\src\library\Uno.Material\Styles\Controls\v2\ContentDialog.xaml";
		var xaml = File.ReadAllText(path);
	}

	private static string FindGitRootFolder(string path)
	{
		var fragments = path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
		var gitRoot = Enumerable.Range(1, fragments.Length - 1)
			.Reverse()
			.Select(x => Path.Combine(fragments[0..x]))
			.FirstOrDefault(x => Directory.Exists(Path.Combine(x, ".git")));
		
		return gitRoot;
	}
	private static Dictionary<string, string> ParseMaterialV2Aliases(string path)
	{
		var source = File.ReadAllText(path);
		var keys = Regex.Matches(source, @"(?<=Add\("")Material\w+(?="")")
			.Select(x => x.Value)
			.ToArray();
		// note: in themes, it is done with `x.Substring("M3Material".Length)`... lets not do that
		var aliases = keys.ToDictionary(x => Regex.Replace(x, "^Material", ""));
		
		return aliases;
	}
	private static void CrawlResourceDependencies(ProjectResourceContextProvider context, string focusIntoFile, bool sort = true)
	{
		focusIntoFile.Dump(PathHelper.MakeRelative(focusIntoFile, context.BasePath));
		
		if (focusIntoFile.EndsWith("xaml.cs"))
			focusIntoFile = focusIntoFile[..^3];
		
		var xamls = context.GetResources()
			.Where(x => x.FullPath != focusIntoFile)
			.Append(new(focusIntoFile, PathHelper.MakeRelative(focusIntoFile, context.BasePath), SourceRelation.Focus))
			.ToArray();

		var definitions = xamls
			.SelectMany(x => GetResourceDefinitions(File.ReadAllText(x.FullPath)).Select(y => new DefinitionSource(y.Key, y.IsAlias, x)))
			.ToList();
		var references = xamls
			.SelectMany(x => GetResourceReferences(File.ReadAllText(x.FullPath)).Select(y => new DefinitionSource(y.Key, y.IsAlias, x)))
			.ToArray();
		
		foreach (var alias in context.GetAliases())
			if (definitions.FirstOrDefault(x => x.Key == alias.Key) is { } found)
				definitions.Add(new DefinitionSource(alias.Alias, true, found.Source));

		definitions
			.Where(x => x.Source.Relation == SourceRelation.Focus)
			.Select(x => new
			{ 
				x.Key,
				ReferenceCount = Util.HighlightIf(references.Count(y => y.Key == x.Key), y => y == 0),
				x.Source.RelativePath
			})
			.Dump("Definitions", 0);
			
		var temp0 = references
			.Where(x => x.Source.Relation == SourceRelation.Focus)
			.GroupBy(x => x, (k, g) => new { Reference = k, Count = g.Count(), Definition = definitions.LastOrDefault(x => x.Key == k.Key) ?? new DefinitionSource(k.Key, false, SourceFile.Missing) })
			.ApplyIf(sort, source => source
				.OrderByDescending(x => x.Definition.Source.Relation)
				.ThenBy(x => x.Definition?.Source.RelativePath)
				.ThenBy(x => x.Reference.Key)
			)
			.ToArray();
		Util.VerticalRun(
			string.Join(", ", temp0.OrderBy(x => x.Definition.Source.Relation).GroupBy(x => x.Definition.Source.Relation, (k, g) => $"{k}: {g.Count()}")),
			temp0
				.Where(x => !x.Definition.IsAlias)
				.Select(x => new
				{
					Key = x.Reference.IsAlias ? $"(alias) -> {x.Reference.Key}" : x.Reference.Key,
					x.Count,
					OriginType = Util.WithStyle(x.Definition.Source.Relation, x.Definition.Source.Relation switch
					{
						SourceRelation.Focus => "",
						SourceRelation.Local => "",
						SourceRelation.Project => "background: green",
						SourceRelation.Foreign => "background: olive",
						
						_ => "background: yellow; color: red",
					}),
					Origin = x.Definition.Source != SourceFile.Missing ? (x.Definition.Source.SourceName?.Apply(y => $"[{y}] ") + x.Definition.Source.RelativePath) : default,
					UsedIn = x.Reference.Source.RelativePath,
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
	public static IEnumerable<string> GetResourceDefinitions0(string xaml)
	{
		return Regex.Matches(xaml, @"x:Key=""(?<key>[^""]+)""(\s+ResourceKey=""(?<alias>[^""]+)"")?")
			.Cast<Match>()
			.Select(x => x.Groups["key"].Value);
	}
	public static IEnumerable<(string Key, bool IsAlias)> GetResourceDefinitions(string xaml)
	{
		return Regex.Matches(xaml, @"x:Key=""(?<key>[^""]+)""(\s+ResourceKey=""(?<alias>[^""]+)"")?")
			.Cast<Match>()
			.Select(x => (x.Groups["key"].Value, x.Groups["alias"].Success));
	}
	public static IEnumerable<string> GetResourceReferences0(string xaml)
	{
		return Regex.Matches(xaml, @"{(Static|Theme)Resource (?<key>[^}]+)}")
			.Concat(Regex.Matches(xaml, @"ResourceKey=""(?<key>\w+)"""))
			.Cast<Match>()
			.Select(x => x.Groups["key"].Value);
	}
	public static IEnumerable<(string Key, bool IsAlias)> GetResourceReferences(string xaml)
	{
		return Regex.Matches(xaml, @"(?<reference>{(Static|Theme)Resource (?<key>[^}]+)})|(?<alias>ResourceKey=""(?<key>\w+)"")")
			.Cast<Match>()
			.Select(x => (x.Groups["key"].Value, x.Groups["alias"].Success));
	}
}

// You can define other methods, fields, classes and namespaces here
public enum SourceRelation { Focus, Local, Project, Foreign, Unknown }

public interface IResourceContextProvider
{
	IEnumerable<SourceFile> GetResources();
}
public class PatternResourceContextProvider : IResourceContextProvider
{
	private readonly Func<string, IEnumerable<SourceFile>> GetResourcesImpl;
	public string Pattern { get; }
	
	public PatternResourceContextProvider(string pattern, Func<string, IEnumerable<SourceFile>> getResources)
	{
		this.GetResourcesImpl = getResources;
		this.Pattern = pattern;
	}
	
	public IEnumerable<SourceFile> GetResources() => GetResourcesImpl(Pattern);
}
public class ProjectResourceContextProvider : IResourceContextProvider
{
	public string BasePath { get; }
	public List<(SourceRelation Relation, string? Name, IResourceContextProvider Provider)> Includes { get; } = new();
	public Dictionary<string, string> Aliases { get; } = new();
	
	public ProjectResourceContextProvider(string basePath)
	{
		this.BasePath = basePath;
	}
	public static implicit operator ProjectResourceContextProvider(string basePath)
	{
		return new ProjectResourceContextProvider(basePath).Include("*");
	}
	
	public ProjectResourceContextProvider Include(string pattern) => Include(SourceRelation.Local, null, pattern);
	public ProjectResourceContextProvider Include(SourceRelation sourceRelation, string pattern) => Include(sourceRelation, null, pattern);
	public ProjectResourceContextProvider Include(SourceRelation sourceRelation, string? sourceName, string pattern)
	{
		return Include(sourceRelation, sourceName, new PatternResourceContextProvider(pattern, x => MatchPattern(x, sourceRelation, sourceName)));
	}
	public ProjectResourceContextProvider Include(SourceRelation sourceRelation, string? sourceName, string basePath, Func<ProjectResourceContextProvider, ProjectResourceContextProvider> build)
	{
		return Include(sourceRelation, sourceName, build(new ProjectResourceContextProvider(basePath)));
	}
	public ProjectResourceContextProvider Include(SourceRelation sourceRelation, string? sourceName, IResourceContextProvider provider)
	{
		Includes.Add((sourceRelation, sourceName, provider));
		
		return this;
	}
	
	public ProjectResourceContextProvider WithAliases(Dictionary<string, string> aliases)
	{
		foreach (var (key, value) in aliases)
		{
			if (!Aliases.TryAdd(key, value))
			{
				Util.Metatext($"Duplicated alias key: {key} -> ({Aliases[key]} => {value})");
				Aliases[key] = value;
			}
		}
		
		return this;
	}

	public IEnumerable<SourceFile> GetResources()
	{
		foreach (var include in Includes)
		{
			foreach (var item in include.Provider.GetResources())
			{
				yield return item with
				{
					Relation = include.Relation,
					SourceName = include.Name
				};
			}
		}
	}
	public IEnumerable<(string Alias, string Key)> GetAliases() // fixme: should be processed on project level, not aggregated
	{
		foreach (var include in Includes)
		{
			if (include.Provider is ProjectResourceContextProvider project)
			{
				foreach (var item in project.GetAliases())
				{
					yield return item;
				}
			}
		}

		foreach (var alias in Aliases)
		{
			yield return (alias.Key, alias.Value);
		}
	}
	
	public IEnumerable<SourceFile> MatchPattern(string pattern, SourceRelation sourceRelation, string? sourceName = null)
	{
		return MatchPattern(pattern).Select(x => new SourceFile(
			x, 
			PathHelper.MakeRelative(x, BasePath), 
			sourceRelation, 
			sourceName
		));
	}
	private IEnumerable<string> MatchPattern(string pattern)
	{
		pattern = PathHelper.UnifyDirectorySeparator(Path.Combine(BasePath, pattern));
		if (pattern.Contains("*")) // handle pre-defined wildcard cases
		{
			// just wildcard filename: * OR *.ext
			if (Regex.Match(pattern, @"^(?<path>[^*]+\\)(?<pattern>\*(\.\w+)?)$") is Match { Success: true } m0)
			{
				return Directory.GetFiles(m0.Groups["path"].Value, m0.GetCapture("pattern") ?? "*");
			}
			// wildcard path and/or no/normal/wildcard filename: **\ OR **\* OR **\fixed.name
			else if (Regex.Match(pattern, @"^(?<path>[^*]+\\)\*\*\\(?<pattern>(\*|\w+)(\.\w+)?)?$") is Match { Success: true } m1)
			{
				return DirectoryHelper
					.EnumerateAllNestedDirectories(m1.Groups["path"].Value, NotGenerated)
					.Prepend(m1.Groups["path"].Value)
					.Where(Directory.Exists)
					.SelectMany(x => Directory.GetFiles(x, m1.GetCapture("pattern") ?? "*"));
			}
			else
			{
				throw new ArgumentException($"Invalid pattern: {pattern}");
			}
		}
		else if (File.Exists(pattern))
		{
			return new[] { pattern };
		}
		else if (Directory.Exists(pattern))
		{
			return DirectoryHelper
				.EnumerateAllNestedDirectories(pattern, NotGenerated)
				.Prepend(pattern)
				.SelectMany(x => Directory.GetFiles(x));
		}
		else
		{
			return Enumerable.Empty<string>();
		}
		
		bool NotGenerated(string directoryPath) =>  ".git,.vs,bin,obj".Split(',').All(x => !directoryPath.EndsWith(x));
	}
}

public record SourceFile(string FullPath, string RelativePath, SourceRelation Relation, string? SourceName = null)
{
	public static SourceFile Missing = new(null!, null!, SourceRelation.Unknown, "missing");
}
public record DefinitionSource(string Key, bool IsAlias, SourceFile Source);

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
	public static string? GetCapture(this Match m, string name)
	{
		return m.Groups[name] is { Success: true} g ? g.Value : default;
	}
}