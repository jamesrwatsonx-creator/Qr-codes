using System.Diagnostics;
using System.Reflection;
using Flink;
using Flink.ShortLinks.Ports;
using Flink.ShortLinks.Repositories;
using Microsoft.Extensions.Logging.Console;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
  .AddJsonFile("appsettings.defaults.json", true, true)
  .AddJsonFile("appsettings.json", false, true)
  .AddCommandLine(args)
  .AddEnvironmentVariables();

builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(o => {
  o.UseUtcTimestamp = true;
  o.IncludeScopes = true;
  o.SingleLine = true;
  o.ColorBehavior = LoggerColorBehavior.Enabled;
  o.TimestampFormat = "yyyy-MM-ddTHH:mm:ss ";
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o => {
  o.SwaggerDoc("v1", CreateOpenApiInfo());
  o.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});

builder.Services.AddScoped<DatabaseContext>();
builder.Services.AddScoped<IShortLinkRepository, SqliteShortLinkRepository>();

builder.Services.AddOpenTelemetry()
  .ConfigureResource(resource => resource.AddService(serviceName: builder.Environment.ApplicationName))
  .WithMetrics(metrics => metrics
    .AddAspNetCoreInstrumentation()
    .AddMeter("Microsoft.AspNetCore.Hosting")
    .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
    .AddPrometheusExporter())
  .WithTracing(tracing => {
    tracing.AddAspNetCoreInstrumentation();
    tracing.AddHttpClientInstrumentation();
    tracing.AddConsoleExporter();
  });

var app = builder.Build();

using (var scope = app.Services.CreateScope()) {
  await scope.ServiceProvider.GetRequiredService<DatabaseContext>().Initialize();
}

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.MapPrometheusScrapingEndpoint();

app.UseHttpsRedirection();
app.Run();
return;

OpenApiInfo CreateOpenApiInfo() {
  return new OpenApiInfo {
    Contact = new OpenApiContact {
      Email = "flink@rtrace.io",
      Url = new Uri("https://flink.rtrace.io"),
      Name = "flink"
    },
    Description = "Flink is a simple/scalable URL Shortener / Link Shortener",
    License = new OpenApiLicense {
      Name = "MIT",
      Url = new Uri("https://en.wikipedia.org/wiki/MIT_License")
    },
    Title = "Flink",
    Version = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion
  };
}
