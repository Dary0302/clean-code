using Markdown.Closer;
using Markdown.MarkdownToken;
using Markdown.Tags;

namespace Markdown;

public class Md
{
    private readonly Dictionary<string, Tag> tagSigns;
    private readonly ParserMd parserMd;
    private readonly TagClosure tagClosure;

    public Md()
    {
        tagSigns = new Dictionary<string, Tag>();
        AddTagsToTagSigns();
        parserMd = new ParserMd(tagSigns);
        tagClosure = new TagClosure(tagSigns);
    }

    public string Render(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        var tokens = parserMd.Parse(text).ToList();
        tokens = tagClosure.Close(tokens);
        var result = GetTokenInterpretation(tokens);

        return string.Join(string.Empty, result);
    }

    private void AddTagsToTagSigns()
    {
        AddTag(new Tag("#", "h1", false));
        AddTag(new Tag("##", "h2", false));
        AddTag(new Tag("###", "h3", false));
        AddTag(new Tag("####", "h4", false));
        AddTag(new Tag("#####", "h5", false));
        AddTag(new Tag("######", "h6", false));
        AddTag(new Tag("_", "<em>", true));
        AddTag(new Tag("__", "<strong>", true));
    }

    private void AddTag(Tag tag)
    {
        tagSigns.Add(tag.MdTag, tag);
    }
    
    public static IEnumerable<string> GetTokenInterpretation(IEnumerable<Token> tokens)
    {
        return tokens.Select(token => token.Type == TokenType.Tag && token.IsClosed ? RenderHtmlTag(token) : token.Value);
    }
    
    private static string RenderHtmlTag(Token token) =>
        token.TagPosition == TagPosition.Opening ? token.Tag.HtmlTag : token.Tag.HtmlTagClose;
}