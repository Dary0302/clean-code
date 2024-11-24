using Markdown.Extensions;
using Markdown.Tags;

namespace Markdown;

public class Md
{
    private readonly Dictionary<string, Tag> tagSigns;
    private readonly ParserMd parserMd;

    public Md()
    {
        tagSigns = new Dictionary<string, Tag>();
        AddTagsToTagSigns();
        parserMd = new ParserMd(tagSigns);
    }

    public string Render(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        var tokens = parserMd.Parse(text);
        var result = tokens.GetTokenInterpretation();

        return string.Join(string.Empty, result);
    }

    private void AddTagsToTagSigns()
    {
        AddTag(new Tag("# ", "<h1>", true));
        AddTag(new Tag("_", "<em>", true));
        AddTag(new Tag("__", "<strong>", true));
    }

    private void AddTag(Tag tag)
    {
        tagSigns.Add(tag.MdTag, tag);
    }
}