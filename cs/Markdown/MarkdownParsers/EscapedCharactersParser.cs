using Markdown.HtmlTools;
using Markdown.TagInfo;
using Markdown.TokenInfo;

namespace Markdown.MarkdownParsers;

public class EscapedCharactersParser : IMarkdownParser
{
    private const int TagLength = 1;
    private const TagType TagType = TagInfo.TagType.Empty;
    private readonly char[] escapedChars = new char[] { '_', '#', '\\' };

    public IEnumerable<Token> Parse(string text)
    {
        var tokens = new List<Token>();
        tokens.AddRange(FindEscapedTokens(text));

        return tokens.OrderBy(x => x.Position).ToList();
    }

    private IEnumerable<Token> FindEscapedTokens(string text)
    {
        var escapedCharPosition = text.IndexOfAny(escapedChars);

        while (escapedCharPosition >= 0)
        {
            if (Checks.IsEscaped(text, escapedCharPosition))
            {
                yield return BuildEscapeTag(escapedCharPosition);
            }

            escapedCharPosition = text.IndexOfAny(escapedChars, escapedCharPosition + 1);
        }
    }

    private static Token BuildEscapeTag(int i)
    {
        return new Token(TagType, i - 1, Tag.Open, TagLength);
    }
}