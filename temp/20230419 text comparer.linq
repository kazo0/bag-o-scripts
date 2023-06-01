<Query Kind="Statements">
  <Namespace>LINQPad.Controls</Namespace>
</Query>

//var forest = """
//@ normal
//MediaPlayerElement#MediaPlayerElementSample4 // Actual=720x405, HV=Stretch/Stretch, CornerRadius=0, Margin=0, Padding=0, Opacity=1, Visibility=Visible
//@ uno repeat
//MediaPlayerElement#MediaPlayerElementSample4 // Actual=720x403.9464882943144, HV=Stretch/Stretch, CornerRadius=0, Margin=0, Padding=0, Opacity=1, Visibility=Visible
//""";
var forest = File.ReadAllText(@"C:\Users\Xiaoy312\Downloads\12315.tree.txt");
var trees = Regex.Split(forest, "(?=^@)", RegexOptions.Multiline)
	.Where(x => !string.IsNullOrWhiteSpace(x))
	.Select(x => x.TrimEnd())
	.ToDictionary(x => Regex.Match(x, "(?=^@ ?).+").Value)
	.Dump("trees", 0);

SelectBox fromKeySelectBox = null, toKeySelectBox = null;
DumpContainer diffDumpContainer = null;

Util.HorizontalRun(true, 
	(fromKeySelectBox = new SelectBox(SelectBoxKind.DropDown, trees.Keys.ToArray(), onSelectionChanged: UpdateDiff)),
	"compare to",
	(toKeySelectBox = new SelectBox(SelectBoxKind.DropDown, trees.Keys.ToArray(), selectedIndex: Math.Min(1, trees.Count - 1), onSelectionChanged: UpdateDiff))
).Dump();
(diffDumpContainer = new DumpContainer()).Dump();
UpdateDiff(default);

void UpdateDiff(SelectBox sender)
{
	diffDumpContainer.Content = Util.Dif(
		trees[(string)fromKeySelectBox.SelectedOption],
		trees[(string)toKeySelectBox.SelectedOption]
	);
}


