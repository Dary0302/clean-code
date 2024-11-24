using FluentAssertions;

namespace Markdown.Tests;

[TestFixture]
public class MdTest
{
    [TestCase("_Hello, world!_", "<em>Hello, world!</em>")]
    [TestCase("___Hello, world!___", "<em><strong>Hello, world!</strong></em>")]
    [TestCase("#Hello, world!#", "<h1>Hello, world!</h1>")]
    [TestCase("Hello, world!", "Hello, world!")]
    [TestCase("__Hello, world!__", "<strong>Hello, world!</strong>")]
    [TestCase("Внутри __двойного выделения _одинарное_ тоже__ работает", "Внутри <strong>двойного выделения <em>одинарное</em> тоже</strong> работает")]
    [TestCase("_12_3", "_12_3")]
    [TestCase("в ра_зных сл_овах", "в ра_зных сл_овах")]
    [TestCase("____,", "____,")]
    [TestCase("_ подчерки_", "_ подчерки_")]
    
    public void Parse_ShouldReturnListOfTokens_WhenInputStringContainsText(string input, string expected)
    {
        var md = new Md();
        var result = md.Render(input);
        result.Should().Be(expected);
    }
}