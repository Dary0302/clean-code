using FluentAssertions;
using Markdown.Closer;
using Markdown.MarkdownToken;
using Markdown.Tags;

namespace Markdown.Tests;

[TestFixture]
public class TagClosureTest
{
    private Dictionary<string, Tag> tagSigns;

    [SetUp]
    public void Setup()
    {
        var tag1 = new Tag("_", "<em>", true);
        var tag2 = new Tag("__", "<strong>", true);
        var tag3 = new Tag("# ", "h1", true);
        tagSigns = new() { { tag1.MdTag, tag1 }, { tag2.MdTag, tag2 } };
    }

    [Test]
    public void Close_ShouldDetermineThatTagsClosed()
    {
        var tag1 = new TagClosureToken("_", TokenType.Tag, 0, 1) { TagPosition = TagPosition.Opening };
        var tag2 = new Token("I", TokenType.Text);
        var tag3 = new TagClosureToken("_", TokenType.Tag, 3, 4) { TagPosition = TagPosition.Closure };

        var tokens = new List<Token> { tag1, tag2, tag3 };

        var closer = new TagClosure(tagSigns);
        var result = closer.Close(tokens);

        result[0].Value.Should().Be("_");
        result[0].IsClosed.Should().BeTrue();
        result[1].Value.Should().Be("I");
        result[1].IsClosed.Should().BeFalse();
        result[2].Value.Should().Be("_");
        result[2].IsClosed.Should().BeTrue();
    }
}