using Markdown.Closer;
using Markdown.MarkdownToken;
using Markdown.Tags;

namespace Markdown;

public class ParserMd(Dictionary<string, Tag> tagSigns)
{
    private bool previousSymbolIsScreen;

    public IEnumerable<Token> Parse(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        var tokens = GetTokens(text);
        tokens = InspectTokensPosition(tokens);

        var closer = new TagClosure(tagSigns);
        tokens = closer.Close(tokens);

        return tokens;
    }

    private void ScreenNext() => previousSymbolIsScreen = true;

    private List<Token> GetTokens(string text)
    {
        var tokens = new List<Token>();
        foreach (var symbol in text)
        {
            if (symbol == '\\' && !previousSymbolIsScreen)
            {
                ScreenNext();
                continue;
            }
            var tokenType = GetTokenType(symbol.ToString());
            var token = new Token(symbol.ToString(), tokenType);
            tokens.Add(token);
        }

        return tokens;
    }

    private TokenType GetTokenType(string symbol)
    {
        var result = TokenType.Text;
        switch (symbol)
        {
            case " ":
                result = TokenType.WhiteSpace;
                break;
            case "\\" when previousSymbolIsScreen:
                result = TokenType.Text;
                break;
            default:
                {
                    if (tagSigns.ContainsKey(symbol))
                    {
                        result = previousSymbolIsScreen ? TokenType.Text : TokenType.Tag;
                    }
                    break;
                }
        }
        previousSymbolIsScreen = false;

        return result;
    }

    private List<Token> InspectTokensPosition(List<Token> tokens)
    {
        for (var i = 0; i < tokens.Count; ++i)
        {
            var isOpening = IsTagOpening(i, tokens);
            tokens[i] = InspectToken(tokens[i], TagPosition.Opening, isOpening);
        }
        for (var i = tokens.Count - 1; i > -1; --i)
        {
            var isEnclosing = IsTagEnclosing(i, tokens);
            tokens[i] = InspectToken(tokens[i], TagPosition.Closure, isEnclosing);
        }
        foreach (var t in tokens)
        {
            if (t.Type == TokenType.Tag && t.TagPosition == TagPosition.None)
            {
                t.Type = TokenType.Text;
            }
        }
        return tokens;

    }

    private Token InspectToken(
        Token token,
        TagPosition inspectedPosition,
        bool isTokenOfInspectedPosition)
    {
        if (token.Type == TokenType.Tag && isTokenOfInspectedPosition)
        {
            token.MdType = tagSigns[token.Value];
            token.TagPosition = inspectedPosition;
        }
        return token;
    }

    private static bool IsTagOpening(int index, IList<Token> tokens) =>
        index == 0 || tokens[index - 1].Type == TokenType.WhiteSpace
        || tokens[index - 1].TagPosition == TagPosition.Opening;

    private static bool IsTagEnclosing(int index, IList<Token> tokens) =>
        index == tokens.Count - 1 || tokens[index + 1].Type == TokenType.WhiteSpace
        || tokens[index + 1].TagPosition == TagPosition.Closure;
}