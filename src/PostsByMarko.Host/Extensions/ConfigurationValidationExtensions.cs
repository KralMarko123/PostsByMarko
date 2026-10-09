using Microsoft.Extensions.Options;
using PostsByMarko.Host.Application.Configuration;

namespace PostsByMarko.Host.Extensions;

public static class ConfigurationValidationExtensions
{
    public static JwtConfig WithValidatedConfiguration(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IValidateOptions<JwtConfig>, JwtConfigValidator>();
        builder.Services.AddSingleton<IValidateOptions<EmailConfig>, EmailConfigValidator>();
        builder.Services.AddSingleton<IValidateOptions<ApplicationUrlConfig>, ApplicationUrlConfigValidator>();
        builder.Services.AddSingleton<IValidateOptions<DatabaseConfig>, DatabaseConfigValidator>();
        builder.Services.AddOptions<JwtConfig>().BindConfiguration("JwtConfig").ValidateOnStart();
        builder.Services.AddOptions<EmailConfig>().BindConfiguration("EmailConfig").ValidateOnStart();
        builder.Services.AddOptions<ApplicationUrlConfig>().BindConfiguration("ApplicationUrls").ValidateOnStart();
        builder.Services.AddOptions<DatabaseConfig>().BindConfiguration("ConnectionStrings").ValidateOnStart();

        // Authentication and CORS need these values while building the service collection.
        var jwt = builder.Configuration.GetSection("JwtConfig").Get<JwtConfig>() ?? new();
        var result = new JwtConfigValidator().Validate(Options.DefaultName, jwt);
        if (result.Failed)
            throw new OptionsValidationException(Options.DefaultName, typeof(JwtConfig), result.Failures);
        return jwt;
    }
}
