using System.Security.Claims;
using Flink.Authorization.Ports.Models;
using Flink.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace Flink.ShortLinks.Ports.Rbac;

public class CanDeleteShortenedLinkRequirementHandler(IOptions<AuthorizationConfiguration> config) : AuthorizationHandler<CanDeleteShortenedLinkRequirement, ShortenedLink> {
  protected override Task HandleRequirementAsync(
    AuthorizationHandlerContext context,
    CanDeleteShortenedLinkRequirement shortenedLinkRequirement,
    ShortenedLink resource) {
    var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (userId == null)
      return Task.CompletedTask;
    
    if (config.Value.Moderators.Any(x => x.UserId == userId) || config.Value.Administrators.Any(x => x.UserId == userId)) {
      context.Succeed(shortenedLinkRequirement);
      return Task.CompletedTask;
    }

    if (resource.Creator == userId) {
      context.Succeed(shortenedLinkRequirement);
    }

    return Task.CompletedTask;
  }
}
