using Markdown.MarkdownToken;

namespace Markdown.Closer;

public class TagClosureToken : Token
{
    public readonly int Start;
    public readonly int End;

    public TagClosureToken() { }

    public TagClosureToken(string value, TokenType type, int start, int end)
        : base(value, type)
    {
        Start = start;
        End = end;
    }
}