using FluentAssertions;
using Markdown.Tags;

namespace Markdown.Tests;

[TestFixture]
public class MdTest
{
    [TestCase("в_нутр_и слова", "в<em>нутр</em>и слова")]
    [TestCase("__Непарные_ символы", "__Непарные_ символы")]
    [TestCase("# Hello, world!", "h1 Hello, world!")]
    [TestCase("## Hello, world!", "h2 Hello, world!")]
    [TestCase("### Hello, world!", "h3 Hello, world!")]
    [TestCase("#### Hello, world!", "h4 Hello, world!")]
    [TestCase("##### Hello, world!", "h5 Hello, world!")]
    [TestCase("###### Hello, world!", "h6 Hello, world!")]
    [TestCase("_Hello, world!_", "<em>Hello, world!</em>")]
    [TestCase("___Hello, world!___", "<em><strong>Hello, world!</strong></em>")]
    [TestCase("Hello, world!", "Hello, world!")]
    [TestCase("__Hello, world!__", "<strong>Hello, world!</strong>")]
    [TestCase("Внутри __двойного выделения _одинарное_ тоже__ работает", "Внутри <strong>двойного выделения <em>одинарное</em> тоже</strong> работает")]
    [TestCase("_12_3", "_12_3")]
    [TestCase("в ра_зных сл_овах", "в ра_зных сл_овах")]
    [TestCase("____,", "____,")]
    [TestCase("_ подчерки_", "_ подчерки_")]
    
    public void Render_ShouldReturnHTMLString(string input, string expected)
    {
        var md = new Md();
        var result = md.Render(input);
        result.Should().Be(expected);
    }
}