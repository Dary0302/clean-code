using FluentAssertions;
using Markdown.Extensions;
using Markdown.Tags;

namespace Markdown.Tests;

public class TokenExtensionsTest
{
    private Dictionary<string, Tag> tagSigns;
    private ParserMd parserMd;
    
    [SetUp]
    public void Setup()
    {
        var tag1 = new Tag("_", "<em>", true);
        var tag2 = new Tag("__", "<strong>", true);
        tagSigns = new(){{tag1.MdTag, tag1}, {tag2.MdTag, tag2}};
        parserMd = new(tagSigns);
    }
    
    [TestCase("_Italics_", "_")]
    [TestCase("__Strong__", "__")]
    [TestCase("__Strong__ _Italics_", "__", "_")]
    public void GetTokenInterpretation_StringWithDifferentTags_CorrectParse(string text, params string[] tags)
    {
        var tokens = parserMd.Parse(text);
        var result= tokens.GetTokenInterpretation().ToList();
        var sumLengthTags = tags.Sum(tag => tag.Length);
        
        result.Count.Should().Be(text.Length - 2 * (sumLengthTags - tags.Length));
    }
    
    [TestCase("_Italics_", "<em>", "</em>")]
    [TestCase("__Strong__", "<strong>", "</strong>")]
    public void GetTokenInterpretation_StringWithTags_ConvertToCorrectHTMLTags(string text, string openTags, string closeTags)
    {
        var tokens = parserMd.Parse(text);
        var result= tokens.GetTokenInterpretation().ToList();
        
        result[0].Should().Be(openTags);
        result[^1].Should().Be(closeTags);
    }
}