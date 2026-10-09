using Microsoft.Extensions.Options;
using MimeKit;
using MySqlConnector;

namespace PostsByMarko.Host.Application.Configuration;

public sealed class JwtConfigValidator : IValidateOptions<JwtConfig>
{
    public ValidateOptionsResult Validate(string? name, JwtConfig options)
    {
        if (name is not null && name != Options.DefaultName) return ValidateOptionsResult.Skip;
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(options.Secret) || options.Secret.Length < 32)
            errors.Add("JwtConfig:Secret must be supplied securely and contain at least 32 characters.");
        if (options.ValidIssuers is null || options.ValidIssuers.Count == 0 || options.ValidIssuers.Any(string.IsNullOrWhiteSpace))
            errors.Add("JwtConfig:ValidIssuers must contain at least one non-empty issuer.");
        // These values also configure CORS, so they must be browser origins.
        if (options.ValidAudiences is null || options.ValidAudiences.Count == 0 || options.ValidAudiences.Any(value =>
                !ConfigurationUrlValidation.IsHttpUrl(value) || new Uri(value).AbsolutePath != "/" || value.EndsWith('/')))
            errors.Add("JwtConfig:ValidAudiences must contain HTTP(S) origins without paths or trailing slashes.");
        if (options.ExpiresInMinutes <= 0)
            errors.Add("JwtConfig:ExpiresInMinutes must be positive.");
        return errors.Count == 0 ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(errors);
    }
}

public sealed class EmailConfigValidator : IValidateOptions<EmailConfig>
{
    public ValidateOptionsResult Validate(string? name, EmailConfig options)
    {
        if (name is not null && name != Options.DefaultName) return ValidateOptionsResult.Skip;
        if (!options.Enabled) return ValidateOptionsResult.Success;
        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(options.Host) || Uri.CheckHostName(options.Host) == UriHostNameType.Unknown)
            errors.Add("EmailConfig:Host must be a hostname or IP address when email delivery is enabled.");
        if (options.Port is < 1 or > 65535)
            errors.Add("EmailConfig:Port must be between 1 and 65535 when email delivery is enabled.");
        if (!string.IsNullOrWhiteSpace(options.Username) && string.IsNullOrEmpty(options.Password))
            errors.Add("EmailConfig:Password is required when EmailConfig:Username is supplied.");
        if (string.IsNullOrWhiteSpace(options.Username) && !string.IsNullOrEmpty(options.Password))
            errors.Add("EmailConfig:Username is required when EmailConfig:Password is supplied.");
        var sender = string.IsNullOrWhiteSpace(options.SenderAddress) ? options.Username : options.SenderAddress;
        if (string.IsNullOrWhiteSpace(sender) || !MailboxAddress.TryParse(sender, out var mailbox) ||
            !string.Equals(mailbox.Address, sender, StringComparison.Ordinal) || !sender.Contains('@'))
            errors.Add("EmailConfig:SenderAddress (or Username when omitted) must be a single email address.");
        return errors.Count == 0 ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(errors);
    }
}

public sealed class ApplicationUrlConfigValidator : IValidateOptions<ApplicationUrlConfig>
{
    public ValidateOptionsResult Validate(string? name, ApplicationUrlConfig options)
    {
        if (name is not null && name != Options.DefaultName) return ValidateOptionsResult.Skip;
        var errors = new List<string>();
        if (!ConfigurationUrlValidation.IsHttpUrl(options.ApiBaseUrl))
            errors.Add("ApplicationUrls:ApiBaseUrl must be an absolute HTTP(S) URL without credentials, a query, or a fragment.");
        if (!ConfigurationUrlValidation.IsHttpUrl(options.ClientBaseUrl))
            errors.Add("ApplicationUrls:ClientBaseUrl must be an absolute HTTP(S) URL without credentials, a query, or a fragment.");
        return errors.Count == 0 ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(errors);
    }
}

public sealed class DatabaseConfigValidator : IValidateOptions<DatabaseConfig>
{
    public ValidateOptionsResult Validate(string? name, DatabaseConfig options)
    {
        if (name is not null && name != Options.DefaultName) return ValidateOptionsResult.Skip;
        const string error = "ConnectionStrings:DefaultConnection must be a valid MariaDB connection string with a server, database, user, and port between 1 and 65535.";
        if (string.IsNullOrWhiteSpace(options.DefaultConnection)) return ValidateOptionsResult.Fail(error);
        try
        {
            var connection = new MySqlConnectionStringBuilder(options.DefaultConnection);
            return !string.IsNullOrWhiteSpace(connection.Server) && !string.IsNullOrWhiteSpace(connection.Database) &&
                   !string.IsNullOrWhiteSpace(connection.UserID) && connection.Port is >= 1 and <= 65535
                ? ValidateOptionsResult.Success : ValidateOptionsResult.Fail(error);
        }
        catch (Exception exception) when (exception is ArgumentException or FormatException or OverflowException)
        {
            // Parser exception messages can contain connection strings and passwords.
            return ValidateOptionsResult.Fail(error);
        }
    }
}

internal static class ConfigurationUrlValidation
{
    public static bool IsHttpUrl(string? value) =>
        !string.IsNullOrWhiteSpace(value) && value == value.Trim() &&
        Uri.TryCreate(value, UriKind.Absolute, out var uri) &&
        uri.Scheme is "http" or "https" && !string.IsNullOrWhiteSpace(uri.Host) &&
        string.IsNullOrEmpty(uri.UserInfo) && string.IsNullOrEmpty(uri.Query) && string.IsNullOrEmpty(uri.Fragment);
}
