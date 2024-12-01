using Markdown.TagInfo;

namespace Markdown.TokenInfo;

public static class TokensHandler
{
    private static readonly Dictionary<TagType, List<TagType>> DisallowedNesting = new()
    {
        { TagType.Italic, [TagType.Strong] },
    };

    public static IEnumerable<Token> Handle(IEnumerable<Token> tokens)
    {
        var selected = new List<Token>();
        var stack = new Stack<Token>();

        foreach (var token in tokens.OrderBy(x => x.Position))
        {
            if (token.TagType == TagType.Empty)
            {
                selected.Add(token);
            }
            else if (token.Tag == Tag.Open)
            {
                stack.Push(token);
            }
            else
            {
                var (opening, closing) = TryGetPair(stack, token);
                selected.Add(opening);
                selected.Add(closing);
            }
        }

        return selected.FindAll(x => x != null).OrderBy(x => x.Position);
    }

    private static bool IsAllowedNesting(Token parent, Token nested)
    {
        if (parent.TagType == TagType.Empty)
        {
            return true;
        }

        if (!DisallowedNesting.TryGetValue(parent.TagType, out var disallowed))
        {
            return true;
        }

        return !disallowed.Contains(nested.TagType);
    }

    private static (Token, Token) TryGetPair(Stack<Token> opened, Token closing)
    {
        if (opened.Count == 0)
        {
            return (null, null)!;
        }

        var opening = opened.Pop();

        if (opening.TagType != closing.TagType)
        {
            return (null, null)!;
        }

        if (opened.Count == 0 || IsAllowedNesting(opened.Peek(), opening))
        {
            return (opening, closing);
        }

        return (null, null)!;
    }
}