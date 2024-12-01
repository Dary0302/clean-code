using Markdown.HtmlTools;
using Markdown.TagInfo;
using Markdown.TokenInfo;

namespace Markdown.MarkdownParsers;

public class PairedTagsParser(string tag, TagType type) : IMarkdownParser
{
    private readonly int tagLength = tag.Length;

    public IEnumerable<Token> Parse(string text)
    {
        var tokens = new List<Token>();
        var openedTags = new Stack<Candidate>();
        var startIndex = 0;

        do
        {
            var foundTagIndex = IndexOfFirstUnescapedTag(text, startIndex);

            if (foundTagIndex == -1)
            {
                break;
            }

            startIndex = foundTagIndex + tag.Length;
            HandleFoundTagAtIndex(foundTagIndex, text, openedTags, tokens);
        }
        while (startIndex < text.Length);

        return tokens;
    }

    private int IndexOfFirstUnescapedTag(string text, int startIndex)
    {
        var firstTagStartIndex = text.IndexOf(tag, startIndex, StringComparison.Ordinal);

        if (firstTagStartIndex == -1)
        {
            return -1;
        }

        var next = text.IndexOf(tag, firstTagStartIndex + tagLength, StringComparison.Ordinal);

        if (Checks.IsEscaped(text, firstTagStartIndex) || (next != -1 && next - firstTagStartIndex == tagLength))
        {
            return IndexOfFirstUnescapedTag(text, firstTagStartIndex + tagLength + 1);
        }

        return firstTagStartIndex;
    }

    private void HandleFoundTagAtIndex(int position, string text, Stack<Candidate> openedTags, List<Token> tokens)
    {
        var candidate = new Candidate
        {
            Position = position,
            EdgeType = openedTags.Count > 0
                ? GetClosingTagState(text, position)
                : GetOpeningTagState(text, position)
        };

        if (candidate.EdgeType == EdgeType.Bad)
        {
            return;
        }

        if (openedTags.Count > 0)
        {
            var opened = openedTags.Pop();

            if (candidate.Position - opened.Position - tagLength > 0)
            {
                tokens.AddRange(TryCreateTagsPair(text, opened, candidate));
            }
        }
        else
        {
            openedTags.Push(candidate);
        }
    }

    private List<Token> TryCreateTagsPair(string text, Candidate open, Candidate close)
    {
        if (Checks.CanSelect(text, open, close))
        {
            return
            [
                new Token(type, open.Position, Tag.Open, tagLength),
                new Token(type, close.Position, Tag.Close, tagLength)
            ];
        }

        return [];
    }

    private static EdgeType GetOpeningTagState(string text, int position)
    {
        if (Checks.IsBeforeSpace(text, position))
        {
            return EdgeType.Bad;
        }

        if (Checks.IsAfterSpace(text, position))
        {
            return EdgeType.Edge;
        }

        return EdgeType.Middle;
    }

    private static EdgeType GetClosingTagState(string text, int position)
    {
        if (Checks.IsAfterSpace(text, position))
        {
            return EdgeType.Bad;
        }

        if (Checks.IsBeforeSpace(text, position))
        {
            return EdgeType.Edge;
        }

        return EdgeType.Middle;
    }

    public struct Candidate
    {
        public int Position;
        public EdgeType EdgeType;
    }

    public enum EdgeType
    {
        Bad,
        Edge,
        Middle
    }
}