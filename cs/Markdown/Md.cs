using Markdown.Parsers;
using Markdown.Renderers;

namespace Markdown;

public class Md
{
    public string Render(string markdown)
    {
        var markdownWithoutEscapes = EscapeHandler.GetMarkdownWithoutEscapes(markdown);
        var tokens = ParserMd.Parse(markdownWithoutEscapes);
        var htmlContent = HtmlRenderer.RenderHtml(tokens);
        return htmlContent;
    }
}