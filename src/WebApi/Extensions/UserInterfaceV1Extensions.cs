using Genocs.CleanArchitecture.Template.WebApi.UseCases.V1.CloseAccount;

namespace Genocs.CleanArchitecture.Template.WebApi.Extensions;

public static class UserInterfaceV1Extensions
{
    public static IServiceCollection AddPresentersV1(this IServiceCollection services)
    {
        services.AddScoped<CloseAccountPresenter, CloseAccountPresenter>();
        services.AddScoped<Application.Boundaries.CloseAccount.IOutputPort>(x => x.GetRequiredService<CloseAccountPresenter>());
        return services;
    }
}
