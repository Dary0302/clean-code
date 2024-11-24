using Markdown.MarkdownToken;

namespace Markdown.Closer;

public class TagClosureToken(string value, TokenType type, int start, int end)
    : Token(value, type)
{
    public readonly int Start = start;
    public readonly int End = end;
}