using Genocs.CleanArchitecture.Template.WebApi.ApiClient;
using Genocs.CleanArchitecture.Template.WebApi.Extensions;
using Microsoft.ApplicationInsights.DependencyCollector;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Refit;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .WriteTo.Console());

// Get services and config
var services = builder.Services;

services.AddApplicationInsightsTelemetry();

services.ConfigureTelemetryModule<DependencyTrackingTelemetryModule>((module, _) =>
{
    module.IncludeDiagnosticSourceActivities.Add("MassTransit");
});

services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.Configure<HealthCheckPublisherOptions>(options =>
{
    options.Delay = TimeSpan.FromSeconds(2);
    options.Predicate = check => check.Tags.Contains("ready");
});

// Setup Cors
services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.WithOrigins(
                            "https://localhost:5001",
                            "http://localhost:5000")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
    });
});

// Register the Swagger generator, defining 1 or more Swagger documents
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "RiftData.Core",
        Description = "Where Data become facts.",
        Contact = new OpenApiContact
        {
            Name = "Rift Data Team",
            Email = "riftdata.gg@gmail.com",
            Url = new Uri("https://www.genocs.com"),
        },
        License = new OpenApiLicense
        {
            Name = "Use under MIT",
            Url = new Uri("https://opensource.org/license/mit/"),
        }
    });

    c.AddServer(new OpenApiServer() { Url = "https://localhost:5001", Description = "Local version for internal test" });

    c.CustomOperationIds(oid =>
    {
        if (!(oid.ActionDescriptor is ControllerActionDescriptor actionDescriptor))
        {
            return null; // default behavior
        }

        return oid.GroupName switch
        {
            "v1" => $"{actionDescriptor.ActionName}",
            _ => $"_{actionDescriptor.ActionName}", // default behavior
        };
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the Text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });

});

services.AddUseCases();

services.AddPresentersV1();

// refit apis
services.AddRefitClient<IOrderApi>()

  // .AddHttpMessageHandler<AuthorizationMessageHandler>()
  .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration["ExternalWebServices:Order"]));

var app = builder.Build();

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseCookiePolicy();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.UseRouting();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();

Log.CloseAndFlush();
