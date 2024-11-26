using Markdown.Closer;
using Markdown.MarkdownToken;
using Markdown.Tags;

namespace Markdown;

public class ParserMd(Dictionary<string, Tag> tagSigns)
{
    public IEnumerable<Token> Parse(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        var tokens = GetTokens(text);
        tokens = InspectTokensPosition(tokens);

        return tokens;
    }

    private List<Token> GetTokens(string text)
    {
        var tokens = new List<Token>();
        var previousSymbolIsEscape = false;
        
        foreach (var symbol in text)
        {
            if (symbol == '\\' && !previousSymbolIsEscape)
            {
                previousSymbolIsEscape = true;
                continue;
            }
            var tokenType = GetTokenType(symbol.ToString(), previousSymbolIsEscape);
            var token = new Token(symbol.ToString(), tokenType);
            tokens.Add(token);
            previousSymbolIsEscape = false;
        }

        return tokens;
    }

    private TokenType GetTokenType(string symbol, bool previousSymbolIsScreen)
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
        foreach (var token in tokens)
        {
            if (token.Type == TokenType.Tag && token.TagPosition == TagPosition.None)
            {
                token.Type = TokenType.Text;
            }
        }
        return tokens;

    }

    private Token InspectToken(
        Token token,
        TagPosition inspectedPosition,
        bool isTokenOfInspectedPosition)
    {
        var newToken = new Token(token);
        
        if (newToken.Type == TokenType.Tag && isTokenOfInspectedPosition)
        {
            newToken.Tag = tagSigns[newToken.Value];
            newToken.TagPosition = inspectedPosition;
        }
        
        return newToken;
    }

    private static bool IsTagOpening(int index, IList<Token> tokens) =>
        index == 0 || tokens[index - 1].Type == TokenType.WhiteSpace || tokens[index - 1].Type == TokenType.Text
        || tokens[index - 1].TagPosition == TagPosition.Opening;

    private static bool IsTagEnclosing(int index, IList<Token> tokens) =>
        index == tokens.Count - 1 || tokens[index + 1].Type == TokenType.WhiteSpace || tokens[index + 1].Type == TokenType.Text
        || tokens[index + 1].TagPosition == TagPosition.Closure;
}