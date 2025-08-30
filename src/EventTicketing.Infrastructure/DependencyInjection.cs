using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using EventTicketing.Application.Common.Interface.Authentication;
using EventTicketing.Application.Common.Interface.Persistence;
using EventTicketing.Infrastructure.Authentication;
using EventTicketing.Application.Common.Interface.Services;
using EventTicketing.Infrastructure.Common.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace EventTicketing.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddScoped<IUserRepository, UserRepository>();
        // Register other infrastructure services here as needed
        return services;
    }

    public static IServiceCollection AddAuthenticationInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind strongly typed settings
        var jwtSettings = configuration.GetSection("Jwt").Get<JWTSettings>();
        if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.SecretKey))
            throw new InvalidOperationException("JWT settings or JWT Key is not configured.");

        services.AddSingleton(jwtSettings); // inject trực tiếp nếu muốn
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
            };
        });

        return services;
    }

}