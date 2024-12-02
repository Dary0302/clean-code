namespace Markdown.HtmlTools;

public static class HtmlLinkTagBuilder
{
    public static string Build(string linkMarkdown)
    {
        var split = linkMarkdown.IndexOf(']');

        if (split <= 0)
        {
            return "";
        }

        var startIndex = split + 2;
        var link = linkMarkdown.Substring(startIndex, linkMarkdown.Length - startIndex - 1);
        var label = linkMarkdown[1..split];

        return $"<a href={link}>{label}</a>";
    }
}