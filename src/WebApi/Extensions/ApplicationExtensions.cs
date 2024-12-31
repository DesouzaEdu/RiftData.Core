using Genocs.CleanArchitecture.Template.Application.UseCases;

namespace Genocs.CleanArchitecture.Template.WebApi.Extensions;

public static class ApplicationExtensions
{
    public static IServiceCollection AddUseCases(this IServiceCollection services)
    {
        services.AddScoped<Application.Boundaries.CloseAccount.IUseCase, CloseAccount>();

        return services;
    }
}
