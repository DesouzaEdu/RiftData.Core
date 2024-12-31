namespace Genocs.CleanArchitecture.Template.WebApi.Extensions;

public static class VersioningExtensions
{
    public static IServiceCollection AddVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(
            options =>
            {

                options.ReportApiVersions = true;
            });

        return services;
    }
}
