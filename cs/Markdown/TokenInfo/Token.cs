using Markdown.TagInfo;

namespace Markdown.TokenInfo;

public class Token(TagType tagType, int position, Tag tag, int tagLength)
{
    public readonly int Position = position;
    public readonly TagType TagType = tagType;
    public readonly Tag Tag = tag;
    public readonly int TagLength = tagLength;
}