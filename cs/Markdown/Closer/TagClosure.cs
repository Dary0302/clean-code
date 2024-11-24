using System.Collections;
using Markdown.MarkdownToken;
using Markdown.Tags;

namespace Markdown.Closer;

public class TagClosure(Dictionary<string, Tag> tagSigns)
{
    public List<Token> Close(List<Token> initialTokens)
    {
        var vacant = new bool[initialTokens.Count];
        var closureTokens = new TagClosureToken[initialTokens.Count];
        SetVacantPlaces(vacant, initialTokens, closureTokens);
        CloseTokens(initialTokens, vacant, closureTokens);

        return GetResult(closureTokens, initialTokens);
    }

    private void EncloseTokensWithSameSign(
        string sign,
        bool[] vacant,
        IList<Token> initialTokens,
        IList<TagClosureToken> closerTokens)
    {
        var currentTokens = initialTokens.Take(sign.Length).ToArray();
        var closing = new Stack<TagClosureToken>();
        for (var i = sign.Length - 1; i < initialTokens.Count - 1; ++i)
        {
            InspectForEnclosing(i, sign, vacant, currentTokens,
                closing, closerTokens);
            ShiftTokens(currentTokens);
            currentTokens[^1] = initialTokens[i + 1];
        }
        InspectForEnclosing(initialTokens.Count - 1, sign, vacant, currentTokens,
            closing, closerTokens);
    }

    private void InspectForEnclosing(
        int i,
        string sign,
        bool[] vacant,
        IList<Token> currentToken,
        Stack<TagClosureToken> closure,
        IList<TagClosureToken> closureTokens)
    {
        var tagString = string.Join("", currentToken.Select(token => token.Value));
        if (IsTag(currentToken, tagString, sign))
        {
            var start = i - sign.Length + 1;
            var end = i + 1;
            var closureToken = new TagClosureToken(tagString, TokenType.Tag, start, end);
            if (!tagSigns[sign].IsClosed)
            {
                ClosureOpenTag(closureToken, closureTokens, vacant);
            }
            else if (IsOpeningTag(currentToken))
            {
                PushTokenForClosing(closureToken, closure);
            }
            else if (IsClosingTag(currentToken, closure))
            {
                ClosePairOfTokens(closureToken, vacant, closure, closureTokens);
            }
        }
    }

    private void CloseTokens(IList<Token> initialTokens, bool[] vacant, IList<TagClosureToken> closerTokens)
    {
        var suitableSigns = tagSigns.Keys
            .OrderByDescending(sign => sign.Length)
            .Where(sign => sign.Length <= initialTokens.Count);
        foreach (var sign in suitableSigns)
        {
            EncloseTokensWithSameSign(sign, vacant, initialTokens, closerTokens);
        }
    }

    private List<Token> GetResult(IList<TagClosureToken> closureTokens, IList<Token> initialTokens)
    {
        var result = new List<Token>();
        var skip = 0;
        for (var i = 0; i < closureTokens.Count; ++i)
        {
            if (skip != 0)
            {
                skip--;
                continue;
            }
            var enclosedToken = closureTokens[i];
            if (enclosedToken == null || initialTokens[i].Type != TokenType.Tag)
            {
                result.Add(initialTokens[i]);
            }
            else
            {
                enclosedToken.MdType = tagSigns[enclosedToken.Value];
                skip = enclosedToken.End - enclosedToken.Start - 1;
                result.Add(enclosedToken);
            }
        }

        return result;
    }

    private static bool IsTag(IEnumerable<Token> tokens, string tagString, string sign) =>
        tagString == sign && !tokens.Any(token => token.Type != TokenType.Tag);

    private static bool IsOpeningTag(IEnumerable<Token> tokens) =>
        !tokens.Any(token => token.TagPosition != TagPosition.Opening);

    private static bool IsClosingTag(IEnumerable<Token> tokens, ICollection closing) =>
        closing.Count != 0 && !tokens.Any(token => token.TagPosition != TagPosition.Closure);

    private static void SetVacantPlaces(
        IList<bool> vacant,
        IList<Token> initialTokens,
        IList<TagClosureToken> closerTokens)
    {
        for (var i = 0; i < initialTokens.Count; ++i)
        {
            if (initialTokens[i].Type == TokenType.Tag)
            {
                vacant[i] = true;
            }
            else
            {
                closerTokens[i] = initialTokens[i] as TagClosureToken;
            }
        }
    }

    private static void ShiftTokens(IList<Token> array)
    {
        for (var i = 0; i < array.Count - 1; ++i)
        {
            array[i] = array[i + 1];
        }
    }

    private static bool AreVacantPlaces(TagClosureToken token, IReadOnlyList<bool> vacant)
    {
        var start = token.Start;
        var end = token.End;
        var isVacant = true;

        for (var i = start; i < end; ++i)
        {
            isVacant = isVacant && vacant[i];
        }

        return isVacant;
    }

    private static void TakeVacantPlaces(TagClosureToken token, IList<bool> vacant)
    {
        var start = token.Start;
        var end = token.End;

        for (var i = start; i < end; ++i)
        {
            vacant[i] = false;
        }
    }

    private static void ClosureOpenTag(
        TagClosureToken openToken,
        IList<TagClosureToken> closureTokens,
        bool[] vacant)
    {
        if (AreVacantPlaces(openToken, vacant))
        {
            openToken.TagPosition = TagPosition.Opening;
            openToken.IsClosed = true;
            closureTokens[openToken.Start] = openToken;
            TakeVacantPlaces(openToken, vacant);
        }
    }

    private static void PushTokenForClosing(TagClosureToken openingToken, Stack<TagClosureToken> closure)
    {
        openingToken.TagPosition = TagPosition.Opening;
        closure.Push(openingToken);
    }

    private static void ClosePairOfTokens(
        TagClosureToken closingToken,
        bool[] vacant,
        Stack<TagClosureToken> closure,
        IList<TagClosureToken> closureTokens)
    {
        var openingToken = closure.Pop();
        if (AreVacantPlaces(closingToken, vacant))
        {
            closingToken.TagPosition = TagPosition.Closure;
            closingToken.IsClosed = true;
            openingToken.IsClosed = true;
            closureTokens[openingToken.Start] = openingToken;
            closureTokens[closingToken.Start] = closingToken;
            TakeVacantPlaces(openingToken, vacant);
            TakeVacantPlaces(closingToken, vacant);
        }
    }
}