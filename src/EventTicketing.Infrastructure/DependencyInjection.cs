
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using EventTicketing.Shared;
using EventTicketing.Application.Services.Authentication.Commands.Register;

namespace EventTicketing.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Register JwtSettings and JwtTokenGenerator for DI
        services.Configure<EventTicketing.Shared.JwtSettings>(options => configuration.GetSection(EventTicketing.Shared.JwtSettings.SectionName).Bind(options));
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
    services.AddScoped<IUserRepository, UserRepository>();
    // Register other infrastructure services here as needed
        return services;
    }

    // Remove AddAuthentication, registration is handled in AddInfrastructure
}