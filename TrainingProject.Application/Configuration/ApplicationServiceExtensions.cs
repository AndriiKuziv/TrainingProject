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
        services.AddValidatorsFromAssemblyContaining(typeof(ApplicationServiceExtensions));
        services.AddScoped<IUserService, UserService>();

        return services;
    }
}
