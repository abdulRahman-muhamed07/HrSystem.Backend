using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HrSystem.Application;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HrSystem.Infrastructure.Security;

public static class JwtAuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration,
        bool isDevelopment)
    {
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .Validate(options => !string.IsNullOrWhiteSpace(options.Key), "Jwt:Key is required.")
            .Validate(options => Encoding.UTF8.GetByteCount(options.Key) >= 32, "Jwt:Key must be at least 32 bytes.")
            .Validate(options => options.ExpirationMinutes is > 0 and <= 60, "Jwt:ExpirationMinutes must be between 1 and 60 minutes.")
            .Validate(options => options.RefreshTokenExpirationDays is >= 1 and <= 30, "Jwt:RefreshTokenExpirationDays must be between 1 and 30 days.")
            .ValidateOnStart();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var jwt = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
                options.RequireHttpsMetadata = !isDevelopment;
                options.SaveToken = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30)
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var jti = context.Principal?.FindFirstValue(JwtRegisteredClaimNames.Jti);
                        if (string.IsNullOrWhiteSpace(jti))
                        {
                            context.Fail("The access token is missing a token identifier.");
                            return;
                        }

                        var revocation = context.HttpContext.RequestServices.GetRequiredService<ITokenRevocationService>();
                        if (await revocation.IsRevokedAsync(jti, context.HttpContext.RequestAborted))
                            context.Fail("The access token has been revoked.");
                    }
                };
            });

        services.AddAuthorization();
        return services;
    }
}
