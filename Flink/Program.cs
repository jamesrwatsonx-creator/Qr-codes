using System.Diagnostics;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using Flink;
using Flink.Authentication.Ports.Models;
using Flink.Authorization.Ports.Models;
using Flink.Embed.Ports;
using Flink.Embed.Ports.IoC;
using Flink.Events.Ports.IoC;
using Flink.Events.Ports.Models;
using Flink.GlobalProxy.Ports.IoC;
using Flink.GlobalProxy.Ports.Models;
using Flink.MetaData.Ports;
using Flink.MetaData.Ports.IoC;
using Flink.Metrics.Ports.Middleware;
using Flink.Metrics.Ports.Models;
using Flink.Migrations;
using Flink.Proxy.Ports.IoC;
using Flink.QRCodes.Ports;
using Flink.QRCodes.Ports.IoC;
using Flink.ShortLinks.Ports;
using Flink.ShortLinks.Ports.IoC;
using Flink.SpamProtection.Ports.IoC;
using Flink.SpamProtection.Ports.Models;
using Flink.WebUI.Ports.IoC;
using Flink.WebUI.Ports.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Console;
using Microsoft.OpenApi;
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
  .ValidateAndConfigureFromRoot<ProxyConfiguration>(out _)
  .ValidateAndConfigure<DbConfiguration>(out var dbConfiguration)
  .ValidateAndConfigure<MetricsConfiguration>(out var metricsConfiguration)
  .ValidateAndConfigure<WebUiConfiguration>()
  .ValidateAndConfigure<SpamProtectionConfiguration>()
  .ValidateAndConfigure<EventNotificationConfiguration>()
  .ValidateAndConfigure<AuthenticationProviderConfiguration>()
  .ValidateAndConfigure<AuthorizationConfiguration>();

var consoleTracingEnabled = builder.Configuration.GetSection("Tracing").GetValue<bool>("ConsoleTracingEnabled");

builder.Services
  .AddDbContext<FlinkDbContext>(ctxOpts => ctxOpts.ConfigureDb(dbConfiguration))
  .RegisterGlobalWebProxy()
  .RegisterShortLinksDomain()
  .RegisterSpamProtection()
  .RegisterMetaDataDomain()
  .RegisterQrCodeDomain()
  .RegisterProxyDomain()
  .RegisterEmbedDomain()
  .RegisterEventsDomain()
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
    .AddMeter(QrCodeMetricsRegistry.MeterName)
    .AddPrometheusExporter())
  .WithTracing(tracing => {
    tracing.AddAspNetCoreInstrumentation();
    tracing.AddHttpClientInstrumentation();
    if (consoleTracingEnabled) {
      tracing.AddConsoleExporter();
    }
  });


var authOptions = builder.Configuration
  .GetSection(nameof(AuthenticationProviderConfiguration))
  .Get<AuthenticationProviderConfiguration>();

var authBuilder = builder.Services.AddAuthentication(options => {
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
  })
  .AddCookie();

foreach (var provider in authOptions!.Providers) {
  switch (provider.AuthProviderType) {
    case AuthProviderType.OpenIdConnect:
      authBuilder.AddOpenIdConnect(provider.Id, options => {
        options.Authority = provider.Authority;
        options.ClientId = provider.ClientId;
        options.ClientSecret = provider.ClientSecret;

        options.ResponseType = "code";
        options.SaveTokens = true;

        foreach (var scope in provider.Scopes ?? new())
          options.Scope.Add(scope);

        options.Events.OnTokenValidated = ctx => {
          var sub = ctx.Principal!.FindFirst("sub")!.Value;

          var identity = (ClaimsIdentity)ctx.Principal.Identity!;
          identity.AddClaim(new Claim(
            ClaimTypes.NameIdentifier,
            $"{provider.Id}:{sub}"
          ));

          return Task.CompletedTask;
        };
      });
      break;
    case AuthProviderType.OAuth:
      authBuilder.AddOAuth(provider.Id, options => {
        options.ClientId = provider.ClientId;
        options.ClientSecret = provider.ClientSecret;

        options.CallbackPath = $"/auth/callback/signin-{provider.Id}";

        options.AuthorizationEndpoint = provider.AuthorizationEndpoint!;
        options.TokenEndpoint = provider.TokenEndpoint!;
        options.UserInformationEndpoint = provider.UserInfoEndpoint!;

        options.SaveTokens = true;

        options.Events = new OAuthEvents {
          OnCreatingTicket = async ctx => {
            var req = new HttpRequestMessage(HttpMethod.Get, ctx.Options.UserInformationEndpoint);
            req.Headers.Authorization =
              new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", ctx.AccessToken);

            var res = await ctx.Backchannel.SendAsync(req);
            res.EnsureSuccessStatusCode();

            using var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync());

            // You may want provider-specific mapping here
            var id = doc.RootElement.GetProperty("id").ToString();

            ctx.Identity!.AddClaim(new Claim(
              ClaimTypes.NameIdentifier,
              $"{provider.Id}:{id}"
            ));
          }
        };
      });
      break;
    default:
      throw new ArgumentOutOfRangeException();
  }
}

var app = builder.Build();

try {
  // dotnet ef migrations add V00xCreateShortenedLinkDbTables --context FlinkDbContext --output-dir Migrations/
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
app.UseForwardedHeaders(new ForwardedHeadersOptions {
  ForwardedHeaders = ForwardedHeaders.XForwardedHost | ForwardedHeaders.XForwardedProto,
});
app.MapControllers();
app.MapScalarApiReference();

app.MapPrometheusScrapingEndpoint(metricsConfiguration.MetricsEndpointPath);
app.UseMiddleware<MetricsMiddleware>();

app.UseWebUi();
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
