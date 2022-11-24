<Query Kind="Program">
  <Namespace>System.Windows</Namespace>
</Query>

const string I18nResource = @"D:\code\uno\platform\Uno.Todo\src\ToDo.UI\Strings\Resources.l12n.xml";
const string OutputPath = @"D:\code\uno\platform\Uno.Todo\src\ToDo.UI\Strings\";
IReadOnlyDictionary<string, string> LanguageMaps =>
	//"en,fr,es".Split(',').ToDictionary(x => x);
	"en,fr".Split(',').ToDictionary(x => x, x => $"{x},default");
	//new Dictionary<string, string> { ["en"] = "fr" };
IEnumerable<string> SubLanguages => LanguageMaps.Values.SelectMany(x => x.Split(',')).Distinct();

void Main()
{
	GenerateResources();
	ShowStringUnderSection("SettingsPage_");
}

void GenerateResources()
{
	var root = XElement.Parse(File.ReadAllText(I18nResource));
	var parsedStrings = root.Descendants()
		.Where(x => x.Name == "data")
		.Select(x => new
		{
			FullName = GetFullName(x),
			Internalization = SubLanguages.ToDictionary(y => y, y =>
				x.Attribute(y)?.Value ??
				x.Element(y)?.Value)
		})
		.ToArray()
		.Dump("parsedString", 0);
	
	parsedStrings.GroupBy(x => x.FullName, (k, g) => new { Key = k, Count = g.Count() }).Where(x => x.Count > 1).Dump("Duplicated Keys", 0);
	//DumpUnbalancedStrings(parsedStrings.ToDictionary(x => x.FullName, x => x.Internalization));

	foreach (var language in LanguageMaps)
	{
		var lookup = language.Value.Split(',').ToArray();
		var result = new XElement("root", Enumerable.Empty<object>()
			.Append(new XComment($"This file is generated from '{Path.GetFileName(I18nResource)}'. Do not modify it manually."))
			.Append(GetReswSchema())
			.Concat(GetReswHeaders())
			.Concat(parsedStrings.Select(x => (object)new XElement("data",
				new XAttribute("name", x.FullName),
				new XAttribute(XNamespace.Xml + "space", "preserve"),
				new XElement("value",
					FindValue(x.Internalization, lookup) ??
					//FindValue(x.Internalization, new[] { "en" }) ??
					throw new KeyNotFoundException($"Could not find internalization resource for '{x.FullName}' in '{language.Key}'")))))
		);

		var path = language.Key.Split('\\') is var parts && parts.Length == 2
			? Path.Combine(OutputPath, parts[0], $"Resources.{parts[1]}.resw")
			: Path.Combine(OutputPath, language.Key, "Resources.resw");

		Directory.CreateDirectory(Path.GetDirectoryName(path));
		File.WriteAllText(path, result.ToString());
		Console.WriteLine($"{parsedStrings.Length} entries written to {path}");
	}
}
void ShowStringUnderSection(string section)
{
	var root = XElement.Parse(File.ReadAllText(I18nResource));
	var parsedStrings = root.Descendants()
		.Where(x => x.Name == "data")
		.Where(x => GetAncestry(x).Any(y => y.Name == "section" && y.Attribute("prefix")?.Value == section))
		.Select(x => new
		{
			FullName = GetFullName(x),
			Internalization = SubLanguages.ToDictionary(y => y, y =>
				x.Attribute(y)?.Value ??
				x.Element(y)?.Value)
		})
		.Select(x => new
		{
			XUid = Clickable.CopyText(x.FullName.Split('.')[0], $"x:Uid=\"{x.FullName.Split('.')[0]}\""),
			Property = x.FullName.Split('.').ElementAtOrDefault(1),
			FullName = x.FullName,
			x.Internalization,
		})
		.ToArray()
		.Dump(1);
}

//object GetResComment()
object GetReswSchema()
{
	return XElement.Parse(@"<xsd:schema id='root' xmlns='' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:msdata='urn:schemas-microsoft-com:xml-msdata'>
		<xsd:import namespace='http://www.w3.org/XML/1998/namespace' />
		<xsd:element name='root' msdata:IsDataSet='true'>
			<xsd:complexType>
				<xsd:choice maxOccurs='unbounded'>
					<xsd:element name='metadata'>
						<xsd:complexType>
							<xsd:sequence>
								<xsd:element name='value' type='xsd:string' minOccurs='0' />
							</xsd:sequence>
							<xsd:attribute name='name' use='required' type='xsd:string' />
							<xsd:attribute name='type' type='xsd:string' />
							<xsd:attribute name='mimetype' type='xsd:string' />
							<xsd:attribute ref='xml:space' />
						</xsd:complexType>
					</xsd:element>
					<xsd:element name='assembly'>
						<xsd:complexType>
							<xsd:attribute name='alias' type='xsd:string' />
							<xsd:attribute name='name' type='xsd:string' />
						</xsd:complexType>
					</xsd:element>
					<xsd:element name='data'>
						<xsd:complexType>
							<xsd:sequence>
								<xsd:element name='value' type='xsd:string' minOccurs='0' msdata:Ordinal='1' />
								<xsd:element name='comment' type='xsd:string' minOccurs='0' msdata:Ordinal='2' />
							</xsd:sequence>
							<xsd:attribute name='name' type='xsd:string' use='required' msdata:Ordinal='1' />
							<xsd:attribute name='type' type='xsd:string' msdata:Ordinal='3' />
							<xsd:attribute name='mimetype' type='xsd:string' msdata:Ordinal='4' />
							<xsd:attribute ref='xml:space' />
						</xsd:complexType>
					</xsd:element>
					<xsd:element name='resheader'>
						<xsd:complexType>
							<xsd:sequence>
								<xsd:element name='value' type='xsd:string' minOccurs='0' msdata:Ordinal='1' />
							</xsd:sequence>
							<xsd:attribute name='name' type='xsd:string' use='required' />
						</xsd:complexType>
					</xsd:element>
				</xsd:choice>
			</xsd:complexType>
		</xsd:element>
	</xsd:schema>".Replace('\'', '"'));
}
IEnumerable<object> GetReswHeaders()
{
	return new (string Name, string Value)[]
	{
		("resmimetype", "text/microsoft-resx"),
		("version", "2.0"),
		("reader", "System.Resources.ResXResourceReader, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"),
		("writer", "System.Resources.ResXResourceWriter, System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"),
	}.Select(x => new XElement("resheader", new XAttribute("name", x.Name), new XElement("value", x.Value)));
}
string GetFullName(XElement data)
{
	return string.Concat(GetAncestry(data.Parent)
		.Reverse()
		.Where(x => x.Name == "section")
		.Select(x => x.Attribute("prefix").Value)
		.Concat(new[] { data.Attribute("name").Value })
	);
}
IEnumerable<XElement> GetAncestry(XElement e)
{
	while (e != null)
	{
		yield return e;
		e = e.Parent;
	}
}
string FindValue(Dictionary<string, string> map, string[] keys)
{
	return keys
		.Select(x => map[x])
		.Where(x => x != null)
		.FirstOrDefault();
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