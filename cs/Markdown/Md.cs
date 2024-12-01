using Markdown.HtmlTools;
using Markdown.MarkdownParsers;
using Markdown.TagInfo;
using Markdown.TokenInfo;

namespace Markdown;

public class Md(IEnumerable<IMarkdownParser> parsers)
{
    public Md() : this([
        new SingleTagParser("# ", TagType.FirstLevelHeader),
        new SingleTagParser("## ", TagType.SecondLevelHeader),
        new SingleTagParser("### ", TagType.ThirdLevelHeader),
        new SingleTagParser("#### ", TagType.FourthLevelHeader),
        new SingleTagParser("##### ", TagType.FifthLevelHeader),
        new SingleTagParser("###### ", TagType.SixthLevelHeader),
        new PairedTagsParser("_", TagType.Italic),
        new PairedTagsParser("__", TagType.Strong),
        new EscapedCharactersParser(),
    ]) { }

    public string Render(string markdownText)
    {
        var tokens = new List<Token>();

        foreach (var parser in parsers)
        {
            tokens.AddRange(parser.Parse(markdownText));
        }

        return HtmlHandler.Handle(markdownText, tokens);
    }
}