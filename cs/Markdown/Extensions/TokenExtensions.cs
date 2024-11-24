using Markdown.MarkdownToken;
using Markdown.Tags;

namespace Markdown.Extensions;

public static class TokenExtensions
{
    public static IEnumerable<string> GetTokenInterpretation(this IEnumerable<Token> tokens)
    {
        return tokens.Select(token => IsTokenTagAndClosed(token) ? GetHtmlTag(token) : token.Value);
    }

    private static bool IsTokenTagAndClosed(Token token) => token is { Type: TokenType.Tag, IsClosed: true };

    private static string GetHtmlTag(Token token) =>
        token.TagPosition == TagPosition.Opening ? token.MdType.HtmlTag : token.MdType.HtmlTagClose;
}