using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace HrSystem.Infrastructure.Security;

public static class JwtAuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        bool isDevelopment)
    {
        services
            .AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.Key), "Jwt:Key is required.")
            .Validate(options => options.Key.Length >= 32, "Jwt:Key must be at least 32 characters.")
            .Validate(options => options.ExpirationMinutes is > 0 and <= 60, "Jwt:ExpirationMinutes must be between 1 and 60 minutes.")
            .Validate(options => options.RefreshTokenExpirationDays is >= 1 and <= 30, "Jwt:RefreshTokenExpirationDays must be between 1 and 30 days.")
            .ValidateOnStart();

        services.AddSingleton<IConfigureOptions<JwtBearerOptions>>(sp =>
            new ConfigureJwtBearerOptions(
                sp.GetRequiredService<IOptions<JwtOptions>>(),
                isDevelopment));

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        return services;
    }
}
