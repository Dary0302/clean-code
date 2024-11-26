using FluentAssertions;
using Markdown.MarkdownToken;
using Markdown.Tags;

namespace Markdown.Tests;

[TestFixture]
public class ParserMdTest
{
    private Dictionary<string, Tag> tagSigns;
    private ParserMd parserMd;

    [SetUp]
    public void Setup()
    {
        var tag1 = new Tag("_", "<em>", true);
        var tag2 = new Tag("__", "<strong>", true);
        tagSigns = new() { { tag1.MdTag, tag1 }, { tag2.MdTag, tag2 } };
        parserMd = new(tagSigns);
    }

    [Test]
    public void Parse_ThrowIfNull()
    {
        Action action = () => parserMd.Parse(null);
        action.Should().Throw<ArgumentNullException>();
    }

    [TestCase("_Italics_", "_")]
    [TestCase("__Strong__", "__")]
    public void Parse_ShouldReturnListOfCorrectTokens(string text, string tag)
    {
        var tokens = parserMd.Parse(text);

        foreach (var token in tokens)
        {
            if (token.Type is not TokenType.Tag)
            {
                continue;
            }
            
            foreach (var symbol in tag)
            {
                token.Value.Should().Be(symbol.ToString());
            }
        }
    }
}