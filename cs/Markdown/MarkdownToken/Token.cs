using Markdown.Tags;

namespace Markdown.MarkdownToken;

public class Token
{
    public string Value { get; }
    public TokenType Type { get; set; }
    public Tag Tag { get; set; }
    public TagPosition TagPosition { get; set; } = TagPosition.None;
    public bool IsClosed { get; set; }

    protected Token() { }

    public Token(string value, TokenType type)
    {
        Value = value;
        Type = type;
    }
    
    public Token(Token token) : this(token.Value, token.Type)
    {
        Tag = token.Tag;
        TagPosition = token.TagPosition;
        IsClosed = token.IsClosed;
    }
}