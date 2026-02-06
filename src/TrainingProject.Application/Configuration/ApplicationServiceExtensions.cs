using Microsoft.Extensions.DependencyInjection;
using TrainingProject.Application.Services;
using TrainingProject.Application.Services.Implementations;
using FluentValidation;
using AutoMapper;

namespace TrainingProject.Application.Configuration;

public static class ApplicationServiceExtensions
{
    public static IServiceCollection AddApplicationServices(
        this IServiceCollection services)
    {
        services.AddAutoMapper(config =>
        {
            config.AddMaps(typeof(ApplicationServiceExtensions).Assembly);
        });
        services.AddValidatorsFromAssembly(typeof(ApplicationServiceExtensions).Assembly, includeInternalTypes: true);
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IValidationService, ValidationService>();

        return services;
    }
}
