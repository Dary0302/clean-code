using Markdown.Tags;

namespace Markdown.MarkdownToken;

public class Token(string value, TokenType type)
{
    public string Value { get; } = value;
    public TokenType Type { get; set; } = type;
    public Tag MdType { get; set; }
    public TagPosition TagPosition { get; set; } = TagPosition.None;
    public bool IsClosed { get; set; }
}