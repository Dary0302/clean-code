using System.Text;
using Markdown.TagInfo;
using Markdown.TokenInfo;

namespace Markdown.HtmlTools;

public static class HtmlHandler
{
    public static string Handle(string inputText, ICollection<Token> tokens)
    {
        var processedText = new StringBuilder(inputText);
        var shiftOffset = 0;

        foreach (var token in TokensHandler.Handle(tokens))
        {
            processedText.Remove(token.Position + shiftOffset, token.TagLength);
            processedText.Insert(token.Position + shiftOffset, GetHtmlTag(token));

            shiftOffset = processedText.Length - inputText.Length;
        }

        return processedText.ToString();
    }

    private static string GetHtmlTag(Token token)
    {
        var htmlTag = token.TagType.GetHtmlOpenTag();

        if (token.Tag == Tag.Close)
        {
            htmlTag = htmlTag.Insert(1, "/");
        }

        return htmlTag;
    }
}