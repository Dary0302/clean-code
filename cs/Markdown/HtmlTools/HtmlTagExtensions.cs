using Markdown.TagInfo;

namespace Markdown.HtmlTools;

public static class HtmlTagExtensions
{
    public static string GetHtmlOpenTag(this TagType tag) => tag switch
    {
        TagType.FirstLevelHeader => "<h1>",
        TagType.SecondLevelHeader => "<h2>",
        TagType.ThirdLevelHeader => "<h3>",
        TagType.FourthLevelHeader => "<h4>",
        TagType.FifthLevelHeader => "<h5>",
        TagType.SixthLevelHeader => "<h6>",
        TagType.Italic => "<em>",
        TagType.Strong => "<strong>",
        var _ => ""
    };
}