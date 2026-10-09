using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PostsByMarko.Host.Application.Configuration;
using PostsByMarko.Host.Extensions;

namespace PostsByMarko.UnitTests;

public class ConfigurationValidationTests
{
    private const string SecretMarker = "configuration-validation-secret";

    [Theory]
    [InlineData("JwtConfig:Secret", "short", "JwtConfig:Secret")]
    [InlineData("JwtConfig:Secret", "                                        ", "JwtConfig:Secret")]
    [InlineData("JwtConfig:ExpiresInMinutes", "0", "JwtConfig:ExpiresInMinutes")]
    [InlineData("JwtConfig:ValidIssuers:0", "", "JwtConfig:ValidIssuers")]
    [InlineData("JwtConfig:ValidAudiences:0", "https://example.test/", "JwtConfig:ValidAudiences")]
    [InlineData("JwtConfig:ValidAudiences:0", "https://example.test/path", "JwtConfig:ValidAudiences")]
    [InlineData("JwtConfig:ValidAudiences:0", "file:///example.test", "JwtConfig:ValidAudiences")]
    [InlineData("ApplicationUrls:ApiBaseUrl", "relative/path", "ApplicationUrls:ApiBaseUrl")]
    [InlineData("ApplicationUrls:ApiBaseUrl", "ftp://example.test", "ApplicationUrls:ApiBaseUrl")]
    [InlineData("ApplicationUrls:ApiBaseUrl", "https://user:configuration-validation-secret@example.test", "ApplicationUrls:ApiBaseUrl")]
    [InlineData("ApplicationUrls:ClientBaseUrl", "https://example.test?token=configuration-validation-secret", "ApplicationUrls:ClientBaseUrl")]
    [InlineData("ApplicationUrls:ClientBaseUrl", "https://example.test#fragment", "ApplicationUrls:ClientBaseUrl")]
    [InlineData("EmailConfig:Host", "", "EmailConfig:Host")]
    [InlineData("EmailConfig:Host", "smtp://example.test", "EmailConfig:Host")]
    [InlineData("EmailConfig:Port", "0", "EmailConfig:Port")]
    [InlineData("EmailConfig:Port", "65536", "EmailConfig:Port")]
    [InlineData("EmailConfig:SenderAddress", "configuration-validation-secret", "EmailConfig:SenderAddress")]
    [InlineData("EmailConfig:SenderAddress", "Display Name <sender@example.test>", "EmailConfig:SenderAddress")]
    [InlineData("EmailConfig:Username", "smtp-login", "EmailConfig:Password")]
    [InlineData("EmailConfig:Password", SecretMarker, "EmailConfig:Username")]
    [InlineData("ConnectionStrings:DefaultConnection", "", "ConnectionStrings:DefaultConnection")]
    [InlineData("ConnectionStrings:DefaultConnection", "server=localhost;user=test;password=configuration-validation-secret", "ConnectionStrings:DefaultConnection")]
    [InlineData("ConnectionStrings:DefaultConnection", "server=localhost;database=test;password=configuration-validation-secret", "ConnectionStrings:DefaultConnection")]
    [InlineData("ConnectionStrings:DefaultConnection", "unsupported=configuration-validation-secret", "ConnectionStrings:DefaultConnection")]
    [InlineData("ConnectionStrings:DefaultConnection", "server=localhost;database=test;user=test;port=0;password=configuration-validation-secret", "ConnectionStrings:DefaultConnection")]
    public void invalid_settings_fail_startup_without_echoing_secrets(string key, string value, string expectedKey)
    {
        var settings = ValidSettings();
        settings[key] = value;
        var exception = Assert.Throws<OptionsValidationException>(() => ValidateStartup(settings));
        Assert.Contains(expectedKey, exception.Message);
        Assert.DoesNotContain(SecretMarker, exception.ToString());
    }

    [Fact]
    public void disabled_email_does_not_require_smtp_settings()
    {
        var settings = ValidSettings();
        settings["EmailConfig:Enabled"] = "false";
        settings["EmailConfig:Host"] = "";
        settings["EmailConfig:Port"] = "0";
        settings["EmailConfig:SenderAddress"] = "";
        ValidateStartup(settings);
    }

    [Fact]
    public void mailpit_without_authentication_and_local_http_urls_are_supported() => ValidateStartup(ValidSettings());

    [Theory]
    [InlineData("sender@example.test", "")]
    [InlineData("smtp-login", "sender@example.test")]
    public void authenticated_smtp_supports_sender_fallback_or_separate_sender(string username, string sender)
    {
        var settings = ValidSettings();
        settings["EmailConfig:Username"] = username;
        settings["EmailConfig:Password"] = SecretMarker;
        settings["EmailConfig:SenderAddress"] = sender;
        ValidateStartup(settings);
    }

    private static void ValidateStartup(Dictionary<string, string?> settings)
    {
        var builder = WebApplication.CreateBuilder(new WebApplicationOptions
        {
            EnvironmentName = "Test", ContentRootPath = AppContext.BaseDirectory
        });
        builder.Configuration.Sources.Clear();
        builder.Configuration.AddInMemoryCollection(settings);
        builder.WithValidatedConfiguration();
        using var app = builder.Build();
        app.Services.GetRequiredService<IStartupValidator>().Validate();
    }

    private static Dictionary<string, string?> ValidSettings() => new()
    {
        ["JwtConfig:Secret"] = "configuration-unit-tests-only-signing-key",
        ["JwtConfig:ValidIssuers:0"] = "http://localhost:7171",
        ["JwtConfig:ValidAudiences:0"] = "http://localhost:3000",
        ["JwtConfig:ExpiresInMinutes"] = "100",
        ["ApplicationUrls:ApiBaseUrl"] = "http://localhost:7171",
        ["ApplicationUrls:ClientBaseUrl"] = "http://localhost:3000",
        ["EmailConfig:Enabled"] = "true",
        ["EmailConfig:Host"] = "mailpit",
        ["EmailConfig:Port"] = "1025",
        ["EmailConfig:SenderAddress"] = "local@postsbymarko.test",
        ["EmailConfig:Username"] = "",
        ["EmailConfig:Password"] = "",
        ["ConnectionStrings:DefaultConnection"] = "server=localhost;database=postsbymarko_test;user=test;password=configuration-validation-secret"
    };
}
