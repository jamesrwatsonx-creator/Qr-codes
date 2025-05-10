using System.Diagnostics;
using System.Reflection;
using Flink;
using Flink.Embed.Ports;
using Flink.Embed.Ports.IoC;
using Flink.MetaData.Ports;
using Flink.MetaData.Ports.IoC;
using Flink.Metrics.Ports.Middleware;
using Flink.Metrics.Ports.Models;
using Flink.Migrations;
using Flink.Proxy.Ports.IoC;
using Flink.QRCodes.Ports.IoC;
using Flink.ShortLinks.Ports;
using Flink.ShortLinks.Ports.IoC;
using Flink.SpamProtection.Ports.IoC;
using Flink.SpamProtection.Ports.Models;
using Flink.WebUI.Ports.IoC;
using Flink.WebUI.Ports.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;
using Microsoft.OpenApi;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
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

builder.Services.AddMemoryCache();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o => {
  o.SwaggerDoc("v1", CreateOpenApiInfo());
  o.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"));
});

builder
  .ValidateAndConfigure<DbConfiguration>(out var dbConfiguration)
  .ValidateAndConfigure<MetricsConfiguration>(out var metricsConfiguration)
  .ValidateAndConfigure<WebUiConfiguration>()
  .ValidateAndConfigure<SpamProtectionConfiguration>();

builder.Services
  .AddDbContext<FlinkDbContext>(ctxOpts => ctxOpts.ConfigureDb(dbConfiguration))
  .RegisterShortLinksDomain()
  .RegisterSpamProtection()
  .RegisterMetaDataDomain()
  .RegisterQrCodeDomain()
  .RegisterProxyDomain()
  .RegisterEmbedDomain()
  .RegisterWebUi();

builder.Services.AddOpenTelemetry()
  .ConfigureResource(resource => resource.AddService(builder.Environment.ApplicationName))
  .WithMetrics(metrics => metrics
    .AddAspNetCoreInstrumentation()
    .AddMeter("Microsoft.AspNetCore.Hosting")
    .AddMeter("Microsoft.AspNetCore.Server.Kestrel")
    .AddMeter(ShortLinkMetricsRegistry.MeterName)
    .AddMeter(ScrapingQueueMetricsRegistry.MeterName)
    .AddMeter(MetaDataMetricsRegistry.MeterName)
    .AddMeter(EmbedMetricsRegistry.MeterName)
    .AddPrometheusExporter())
  .WithTracing(tracing => {
    tracing.AddAspNetCoreInstrumentation();
    tracing.AddHttpClientInstrumentation();
    tracing.AddConsoleExporter();
  });

var app = builder.Build();

try {
  /*
  dotnet ef migrations add V001CreateShortenedLinkDbTables --context ShortenedLinkDbContext --output-dir Migrations/ShortenedLink
  dotnet ef migrations add V001CreateMetaDataDbTables --context MetaDataDbContext --output-dir Migrations/MetaData
  */
  using var scope = app.Services.CreateScope();
  scope.ServiceProvider.GetRequiredService<FlinkDbContext>().Database.Migrate();
} catch {
  /* ignored */
}

app.UseSwagger(x => {
  x.RouteTemplate = "/openapi/{documentName}.json";
  x.OpenApiVersion = OpenApiSpecVersion.OpenApi3_0;
});
app.UseSwaggerUI(x => {
  x.SwaggerEndpoint("/openapi/v1.json", "v1");
});
app.MapControllers();
app.MapScalarApiReference();

app.MapPrometheusScrapingEndpoint(metricsConfiguration.MetricsEndpointPath);
app.UseMiddleware<MetricsMiddleware>();

app.UseWebUi();
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

namespace Flink {
  public abstract class Program;
}
