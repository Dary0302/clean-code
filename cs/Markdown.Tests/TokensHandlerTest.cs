using FluentAssertions;
using Markdown.TagInfo;
using Markdown.TokenInfo;

namespace Markdown.Tests;

[TestFixture]
public class TokensHandlerTest
{
    [Test]
    public void Process_WithoutTokens_ReturnsEmpty()
    {
        var tokens = new List<Token>();

        var processedTokens = TokensHandler.Handle(tokens);

        processedTokens.Should().BeEmpty();
    }

    [Test]
    public void Process_UnpairedTokens_ReturnsEmpty()
    {
        var tokens = new List<Token>
        {
            new Token(TagType.Strong, 1, Tag.Open, 2),
            new Token(TagType.Italic, 3, Tag.Close, 1),
        };

        var processedTokens = TokensHandler.Handle(tokens);

        processedTokens.Should().BeEmpty();
    }

    [Test]
    public void Process_CorrectNesting()
    {
        var tokens = new List<Token>
        {
            new Token(TagType.Strong, 1, Tag.Open, 2),
            new Token(TagType.Italic, 3, Tag.Open, 1),
            new Token(TagType.Italic, 5, Tag.Close, 1),
            new Token(TagType.Strong, 7, Tag.Close, 2),
        };

        var processedTokens = TokensHandler.Handle(tokens);

        processedTokens.Should().HaveCount(4);
    }

    [Test]
    public void Process_InvalidNesting()
    {
        var tokens = new List<Token>
        {
            new Token(TagType.Italic, 1, Tag.Open, 1),
            new Token(TagType.Strong, 2, Tag.Open, 2),
            new Token(TagType.Strong, 5, Tag.Close, 2),
            new Token(TagType.Italic, 7, Tag.Close, 1),
        };

        var processedTokens = TokensHandler.Handle(tokens);

        processedTokens.Should().HaveCount(2);
    }

    [Test]
    public void Process_TagsIntersection_ReturnsEmpty()
    {
        var tokens = new List<Token>
        {
            new Token(TagType.Strong, 1, Tag.Open, 2),
            new Token(TagType.Italic, 3, Tag.Open, 1),
            new Token(TagType.Strong, 5, Tag.Close, 2),
            new Token(TagType.Italic, 7, Tag.Close, 1),
        };

        var processedTokens = TokensHandler.Handle(tokens);

        processedTokens.Should().BeEmpty();
    }
}