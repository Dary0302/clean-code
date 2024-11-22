namespace Markdown.Token;

public class MdToken(MdTokenType mdType, string content, List<MdToken>? children = null)
{
    public MdTokenType MdType { get; set; } = mdType;
    public string Content { get; set; } = content;
    public List<MdToken>? Children { get; set; } = children;
}