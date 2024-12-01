using FluentAssertions;
using Markdown.TagInfo;
using Markdown.TokenInfo;
using Markdown.HtmlTools;

namespace Markdown.Tests;

public class HtmlHandlerTests
{
    [Test]
    public void Handle_WithItalicToken()
    {
        var inputText = " _a_ ";
        var tokens = new List<Token>
        {
            new Token(TagType.Italic, 1, Tag.Open, 1), new Token(TagType.Italic, 3, Tag.Close, 1)
        };

        var result = HtmlHandler.Handle(inputText, tokens);

        result.Should().Be(" <em>a</em> ");
    }

    [Test]
    public void Handle_WithStrongToken()
    {
        var inputText = "% __Hello__ c";
        var tokens = new List<Token>
        {
            new Token(TagType.Strong, 2, Tag.Open, 2), new Token(TagType.Strong, 9, Tag.Close, 2)
        };

        var result = HtmlHandler.Handle(inputText, tokens);

        result.Should().Be("% <strong>Hello</strong> c");
    }

    [Test]
    public void Handle_WithHeaderToken()
    {
        var inputText = "# a";
        var tokens = new List<Token>
        {
            new Token(TagType.FirstLevelHeader, 0, Tag.Open, 2),
            new Token(TagType.FirstLevelHeader, 3, Tag.Close, 0)
        };

        var result = HtmlHandler.Handle(inputText, tokens);

        result.Should().Be("<h1>a</h1>");
    }
}