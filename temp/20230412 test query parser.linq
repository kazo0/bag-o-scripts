<Query Kind="Program">
  <Namespace>Xunit</Namespace>
  <Namespace>System.Diagnostics.CodeAnalysis</Namespace>
  <Namespace>BenchmarkDotNet.Attributes</Namespace>
</Query>

#load "BenchmarkDotNet"

#load "xunit"
#nullable enable

// context: https://github.com/unoplatform/uno.ui.runtimetests.engine/issues/89

void Main()
{
	RunTests();
	
	"""
	listview
	listview measure
	class:listview method:measure -recycle
	c:listview m:measure d:nice_name -recycle
	d:"^asd \", asd$"
	at:asd"asd"asd,^asd,asd$
	""".Split('\n')
		.Where(x => !x.TrimStart().StartsWith("//"))
		.Select(x => new
		{
			x,
			Predicates = SearchPredicate.ParseQuery(x),
		})
		.Dump(0);
}

public class SearchPredicateQueryTests
{
	static IEnumerable<object[]> ParsingTestCases()
	{
		return """
		asd
		asd qwe
		class:asd method:qwe display_name:zxc -123
		c:asd m:qwe d:zxc -123
		d:"^asd \", asd$"
		at:asd"asd"asd,^asd,asd$
		""".Split('\n')
			.Where(x => !x.TrimStart().StartsWith("//"))
			.Select(x => x.Trim())
			.Select(x => new object[] { x });
	}

	[Theory, MemberData(nameof(ParsingTestCases))]
	void Parse(string input)
	{
		var actual = SearchPredicate.ParseQuery(input);
		var expected = input switch
		{
			"asd" => new SearchPredicate[] { new("asd", "asd", Parts: new SearchPredicatePart[] { new("asd", "asd"), }), },
			"asd qwe" => new SearchPredicate[] {
				new("asd", "asd", Parts: new SearchPredicatePart[] { new("asd", "asd"), }),
				new("qwe", "qwe", Parts: new SearchPredicatePart[] { new("qwe", "qwe"), }),
			},
			"class:asd method:qwe display_name:zxc -123" => new SearchPredicate[] { 
				new("class:asd", "asd", Tag: "class", Parts: new SearchPredicatePart[] { new("asd", "asd"), }),
				new("method:qwe", "qwe", Tag: "method", Parts: new SearchPredicatePart[] { new("qwe", "qwe"), }),
				new("display_name:zxc", "zxc", Tag: "display_name", Parts: new SearchPredicatePart[] { new("zxc", "zxc"), }),
				new("-123", "123", Exclusion: true, Parts: new SearchPredicatePart[] { new("123", "123"), }),
			},
			"c:asd m:qwe d:zxc -123" => new SearchPredicate[] { 
				new("c:asd", "asd", Tag: "class", Parts: new SearchPredicatePart[] { new("asd", "asd"), }),
				new("m:qwe", "qwe", Tag: "method", Parts: new SearchPredicatePart[] { new("qwe", "qwe"), }),
				new("d:zxc", "zxc", Tag: "display_name", Parts: new SearchPredicatePart[] { new("zxc", "zxc"), }),
				new("-123", "123", Exclusion: true, Parts: new SearchPredicatePart[] { new("123", "123"), }),
			},
			"d:\"^asd \\\", asd$\"" => new SearchPredicate[] { 
				new("d:\"^asd \\\", asd$\"", "\"^asd \\\", asd$\"", Tag: "display_name", Parts: new SearchPredicatePart[] { 
					new("\"^asd \\\", asd$\"", "asd \", asd", MatchStart: true, MatchEnd: true), 
				}), 
			},
			"at:asd\"asd\"asd,^asd,asd$" => new SearchPredicate[] {
				new("at:asd\"asd\"asd,^asd,asd$", "asd\"asd\"asd,^asd,asd$", Tag: "at", Parts: new SearchPredicatePart[] { 
					new("asd\"asd\"asd", "asd\"asd\"asd"), 
					new ("^asd", "asd", MatchStart: true),
					new ("asd$", "asd", MatchEnd: true),
				}),
			},
			
			_ => Util.Metatext("fixme: replace by throw when all cases are done").Dump() as SearchPredicate[], 
		};
	
		if (!SearchPredicate.DefaultQueryComparer.Equals(expected, actual)) 
			Util.VerticalRun(Util.Metatext("actual=red, expected=green"), Util.Dif(expected, actual)).Dump(input, 0);
		Assert.Equal(expected, actual, SearchPredicate.DefaultQueryComparer);
	}
}
public class SearchPredicateFragmentTests
{
	[Fact]
	void Simple()
	{
		var actual = SearchPredicate.ParseFragment("asd");
		var expected = new SearchPredicate("asd", "asd", Parts: new SearchPredicatePart("asd", "asd"));

		Assert.Equal(expected, actual, SearchPredicate.DefaultComparer);
	}

	[Fact]
	void MultipleParts()
	{
		var actual = SearchPredicate.ParseFragment("^asd,qwe$,^zxc$");
		var expected = new SearchPredicate("^asd,qwe$,^zxc$", "^asd,qwe$,^zxc$", Parts: new[]{
			new SearchPredicatePart("^asd", "asd", MatchStart: true),
			new SearchPredicatePart("qwe$", "qwe", MatchEnd: true),
			new SearchPredicatePart("^zxc$", "zxc", MatchStart: true, MatchEnd: true),
		});

		Assert.Equal(expected, actual, SearchPredicate.DefaultComparer);
	}

	[Fact]
	void Exclusion()
	{
		var actual = SearchPredicate.ParseFragment("-asd");
		var expected = new SearchPredicate("-asd", "asd", Exclusion: true, Parts: new[]{
			new SearchPredicatePart("asd", "asd"),
		});

		Assert.Equal(expected, actual, SearchPredicate.DefaultComparer);
	}

	[Fact]
	void Tagged()
	{
		var actual = SearchPredicate.ParseFragment("tag:asd");
		var expected = new SearchPredicate("tag:asd", "asd", Tag: "tag", Parts: new[]{
			new SearchPredicatePart("asd", "asd"),
		});

		Assert.Equal(expected, actual, SearchPredicate.DefaultComparer);
	}
}
public class SearchPredicatePartTests
{
	[Fact]
	public void Simple()
	{
		var actual = SearchPredicatePart.Parse("asd");
		var expected = new SearchPredicatePart(actual.Raw, "asd");

		Assert.Equal(expected, actual);
	}
	[Fact]
	public void MatchStart()
	{
		var actual = SearchPredicatePart.Parse("^asd");
		var expected = new SearchPredicatePart(actual.Raw, "asd", MatchStart: true);

		Assert.Equal(expected, actual);
	}
	[Fact]
	public void MatchEnd()
	{
		var actual = SearchPredicatePart.Parse("asd$");
		var expected = new SearchPredicatePart(actual.Raw, "asd", MatchEnd: true);

		Assert.Equal(expected, actual);
	}
	[Fact]
	public void FullMatch()
	{
		var actual = SearchPredicatePart.Parse("^asd$");
		var expected = new SearchPredicatePart(actual.Raw, "asd", MatchStart: true, MatchEnd: true);

		Assert.Equal(expected, actual);
	}
}

public record SearchPredicate(string Raw, string Text, bool Exclusion = false, string? Tag = null, params SearchPredicatePart[] Parts)
{
	public static SearchPredicateComparer DefaultComparer = new();
	public static SearchQueryComparer DefaultQueryComparer = new();

	// fixme: domain specific configuration should be injectable
	private static readonly IReadOnlyDictionary<string, string> NamespaceAliases =
		new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
		{
			["c"] = "class",
			["m"] = "method",
			["d"] = "display_name",
		};

	public static SearchPredicate[] ParseQuery(string? query)
	{
		if (string.IsNullOrWhiteSpace(query)) return new SearchPredicate[0];

		return query.SplitWithIgnore(' ', @""".*?(?<!\\)""", skipEmptyEntries: true)
			.Select(ParseFragment)
			.OfType<SearchPredicate>()
			.Where(x => x.Text.Length > 0)
			.ToArray();
	}

	public static SearchPredicate? ParseFragment(string criteria)
	{
		if (string.IsNullOrWhiteSpace(criteria)) return null;

		var raw = criteria.Trim();
		var text = raw;
		if (text.StartsWith('-') is var exclusion && exclusion)
		{
			text = text[1..];
		}
		var tag = default(string?);
		if (text.Split(':', 2) is { Length: 2 } tagParts)
		{
			tag = NamespaceAliases.TryGetValue(tagParts[0], out var value) ? value : tagParts[0];
			text = tagParts[1];
		}
		var parts = text.SplitWithIgnore(',', @""".*?(?<!\\)""", skipEmptyEntries: false)
			.Select(SearchPredicatePart.Parse)
			.ToArray();

		return new(raw, text, exclusion, tag, parts);
	}

	public bool IsMatch(string input) => Parts
		.Any(x => (x.MatchStart, x.MatchEnd) switch
		{
			(true, false) => input.StartsWith(x.Text),
			(false, true) => input.EndsWith(x.Text),

			_ => input.Contains(x.Text),
		});

	public class SearchPredicateComparer : IEqualityComparer<SearchPredicate?>
	{
		public int GetHashCode([DisallowNull] SearchPredicate obj) => obj.GetHashCode();
		public bool Equals(SearchPredicate? x, SearchPredicate? y)
		{
			return (x, y) switch
			{
				(null, null) => true,
				(null, _) => false,
				(_, null) => false,
				_ =>
					x.Raw == y.Raw &&
					x.Text == y.Text &&
					x.Exclusion == y.Exclusion &&
					x.Tag == y.Tag &&
					x.Parts.SequenceEqual(y.Parts),
			};
		}
	}

	public class SearchQueryComparer : IEqualityComparer<SearchPredicate[]?>
	{
		public int GetHashCode([DisallowNull] SearchPredicate[] obj) => obj.GetHashCode();
		public bool Equals(SearchPredicate[]? x, SearchPredicate[]? y)
		{
			return (x, y) switch
			{
				(null, null) => true,
				(null, _) => false,
				(_, null) => false,
				_ => x.SequenceEqual(y, DefaultComparer),
			};
		}
	}
}
public record SearchPredicatePart(string Raw, string Text, bool MatchStart = false, bool MatchEnd = false)
{
	public static SearchPredicatePart Parse(string part)
	{
		var raw = part;
		var text = raw;

		if (text.Length >= 2 && text.StartsWith('"') && text.EndsWith('"'))
		{
			// unquote and unescape \" to "
			text = text
				.Substring(1, text.Length - 2)
				.Replace("\\\"", "\"");
		}
		if (text.StartsWith("^") is { } matchStart && matchStart)
		{
			text = text.Substring(1);
		}
		if (text.EndsWith("$") is { } matchEnd && matchEnd)
		{
			text = text.Substring(0, text.Length - 1);
		}

		return new(raw, text, matchStart, matchEnd);
	}
}

public static class StringExtensions
{
	public static string[] SplitWithIgnore(this string input, char delimiter, string ignoredPattern, bool skipEmptyEntries)
	{
		var ignores = Regex.Matches(input, ignoredPattern);

		var shards = new List<string>();
		for (int i = 0; i < input.Length; i++)
		{
			var nextSpaceDelimitor = input.IndexOf(delimiter, i);

			// find the next space, if inside a quote
			while (nextSpaceDelimitor != -1 && ignores.FirstOrDefault(x => InRange(x, nextSpaceDelimitor)) is { } enclosingIgnore)
			{
				nextSpaceDelimitor = enclosingIgnore.Index + enclosingIgnore.Length + 1 is { } afterIgnore && afterIgnore < input.Length
					? input.IndexOf(delimiter, afterIgnore)
					: -1;
			}

			if (nextSpaceDelimitor != -1)
			{
				shards.Add(input.Substring(i, nextSpaceDelimitor - i));
				i = nextSpaceDelimitor;

				// skip multiple continous spaces
				while (skipEmptyEntries && i + 1 < input.Length && input[i + 1] == delimiter) i++;
			}
			else
			{
				shards.Add(input.Substring(i));
				break;
			}
		}

		return shards.ToArray();

		bool InRange(Match x, int index) => x.Index <= index && index < (x.Index + x.Length);
	}
}