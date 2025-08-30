using BuberDinner.Api.Common.Mapping;
using EventTicketing.API.Configurations;

namespace EventTicketing.API.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddMappings(); 
        return services;
    }
}