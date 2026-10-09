using System.Net;
using System.Text.RegularExpressions;
using PostsByMarko.Host.Application.Helper;

namespace PostsByMarko.UnitTests;

public class ConfirmationEmailTemplateTests
{
    [Fact]
    public void button_and_copyable_link_preserve_the_confirmation_url()
    {
        // Arrange
        const string link = "http://localhost:7171/api/auth/confirm?email=user%2Btest%40example.test&token=a%2Bb%2Fc%3D";

        // Act
        var content = ConfirmationEmailTemplate.Create("Alex", link);
        var links = Regex.Matches(content.HtmlBody, "href=\"([^\"]+)\"");

        // Assert
        Assert.Equal(2, links.Count);
        Assert.All(links.Cast<Match>(), match => Assert.Equal(link, WebUtility.HtmlDecode(match.Groups[1].Value)));
        Assert.Contains("Confirm your email</a>", content.HtmlBody);
        Assert.Contains(link, content.TextBody);
        Assert.Contains("Hi Alex,", content.TextBody);
        Assert.Contains("ignore this email", content.HtmlBody);
        Assert.Contains("ignore this email", content.TextBody);
    }

    [Theory]
    [InlineData("<script>alert('name')</script>")]
    [InlineData("Sam & \"Alex\"")]
    public void names_are_text_and_cannot_inject_html(string name)
    {
        // Arrange
        const string link = "https://example.test/api/auth/confirm?email=user%40example.test&token=token";

        // Act
        var content = ConfirmationEmailTemplate.Create(name, link);
        var greeting = Regex.Match(content.HtmlBody, "Hi (.*?),</p>").Groups[1].Value;

        // Assert
        Assert.Equal(name, WebUtility.HtmlDecode(greeting));
        Assert.DoesNotContain(name, content.HtmlBody);
        Assert.DoesNotContain("<script>", content.HtmlBody);
        Assert.Contains(name, content.TextBody);
    }

    [Fact]
    public void a_link_cannot_escape_its_html_attribute()
    {
        // Arrange
        const string link = "https://example.test/api/auth/confirm?token=\" onmouseover=\"alert(1)&email=user@example.test";

        // Act
        var content = ConfirmationEmailTemplate.Create("Alex", link);
        var links = Regex.Matches(content.HtmlBody, "href=\"([^\"]+)\"");

        // Assert
        Assert.Equal(2, links.Count);
        Assert.All(links.Cast<Match>(), match => Assert.Equal(link, WebUtility.HtmlDecode(match.Groups[1].Value)));
        Assert.DoesNotContain("onmouseover=\"", content.HtmlBody);
    }
}
