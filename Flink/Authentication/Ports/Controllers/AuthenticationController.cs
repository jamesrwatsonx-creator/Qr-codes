using System.Net;
using System.Security.Claims;
using Flink.Authentication.Ports.Models;
using Flink.Shared.ApiResponse;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Flink.Authentication.Ports.Controllers;

[ApiController]
[Produces("application/json")]
public class AuthenticationController(IOptions<AuthenticationProviderConfiguration> authenticationProviderConfiguration) : ControllerBase {
  [HttpGet("/auth/providers")]
  public ApiResponse<List<IdentityProvider>> GetProviders() {
    var providers = authenticationProviderConfiguration.Value.Providers
      .Where(p => p.IsEnabled)
      .Select(p => new IdentityProvider {
        Id = p.Id,
        DisplayName = p.DisplayName,
        LoginUrl = $"/auth/login?provider={p.Id}",
        ImageUrl = p.IconUrl ?? "/static/img/flink-logo.png"
      })
      .ToList();

    return ApiResponse<List<IdentityProvider>>.Create(HttpStatusCode.OK, providers, $"Found {providers.Count} active Identity Providers.");
  }

  [HttpGet("/auth/login")]
  public IActionResult Login(string provider) {
    return Challenge(new AuthenticationProperties { RedirectUri = "/" }, provider);
  }

  [HttpGet("/auth/me")]
  public ApiResponse<SelfIdentity> Me() {
    if (!User.Identity?.IsAuthenticated ?? true)
      return ApiResponse<SelfIdentity>.Create(HttpStatusCode.Unauthorized, null, "You're not logged in.");

    return ApiResponse<SelfIdentity>.Create(HttpStatusCode.OK, new SelfIdentity() {
      UserId = User.FindFirst(ClaimTypes.NameIdentifier)!.Value,
      Name = User.Identity?.Name
    }, "You're logged in.");
  }

  [HttpPost("/auth/logout")]
  public IActionResult Logout() {
    return SignOut();
  }
}
