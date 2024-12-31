using Asp.Versioning.ApiExplorer;
using Genocs.CleanArchitecture.Template.WebApi.ApiClient;
using Genocs.CleanArchitecture.Template.WebApi.Extensions;
using Genocs.CleanArchitecture.Template.WebApi.Extensions.FeatureFlags;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Refit;

namespace Genocs.CleanArchitecture.Template.WebApi;

public sealed class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureDevelopmentServices(IServiceCollection services)
        => InternalConfiguration(services);

    public void ConfigureProductionServices(IServiceCollection services)
        => InternalConfiguration(services);

    public void ConfigureDockerServices(IServiceCollection services)
        => InternalConfiguration(services);

    private void InternalConfiguration(IServiceCollection services)
    {
        services.AddControllers().AddControllersAsServices();
        services.AddBusinessExceptionFilter();
        services.AddFeatureFlags(Configuration);
        services.AddVersioning();
        services.AddSwagger();
        services.AddUseCases();

        services.AddPresentersV1();

        services.AddRefitClient<IOrderApi>()

          .ConfigureHttpClient(c => c.BaseAddress = new Uri(Configuration["ExternalWebServices:Order"]));

    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IApiVersionDescriptionProvider provider)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();
        app.UseVersionedSwagger(provider);
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseCookiePolicy();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }

    public void HealthChecks(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks().AddMongoDb(
            mongodbConnectionString: "mongodb://localhost:27017",
            name: "MongoDB",
            failureStatus: HealthStatus.Unhealthy,
            tags: new string[] { "db", "mongoDB" });
    }
}