namespace Markdown.Tags;

public class Tag(string mdTag, string htmlTag, bool isClosed)
{
    public readonly string MdTag = mdTag;

    public readonly string HtmlTag = htmlTag;
        
    public readonly bool IsClosed = isClosed;

    public readonly string HtmlTagClose = string.Concat(htmlTag[0].ToString(), "/", htmlTag.AsSpan(1, htmlTag.Length - 1));
}