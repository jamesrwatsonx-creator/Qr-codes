using System.Net;
using CSharpFunctionalExtensions;
using Flink.Domain;
using Flink.Events.Ports;
using Flink.MetaData.Ports;
using Flink.Proxy.Ports.Extensions;
using Flink.Shared;
using Flink.Shared.ApiResponse;
using Flink.Shared.Extensions;
using Flink.ShortLinks.Models;
using Flink.ShortLinks.Ports.Events;
using Flink.SpamProtection.Ports.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Flink.ShortLinks.Ports.Controllers;

[ApiController]
[Produces("application/json")]
public class ShortLinkController(
  IShortLinkRepository shortLinkRepository,
  ILogger<ShortLinkController> logger,
  IScrapingQueue scrapingQueue,
  ShortLinkMetricsRegistry metrics,
  IBlockedHostRepository blockedHostRepository,
  IEventNotificationQueue eventNotificationQueue,
  TimeProvider timeProvider)
  : ControllerBase {
  /// <summary>
  ///   Get redirected to the URL of the shortLink
  /// </summary>
  /// <param name="shortLinkId">The id of the previously created shortlink</param>
  /// <param name="password">password to visit a shortlink (only needed if a shortlink was set up with a password)</param>
  /// <returns></returns>
  [HttpGet("/{shortLinkId}", Name = "GetShortLinkRoot")]
  [HttpGet("/links/{shortLinkId}", Name = "GetShortLink")]
  [ProducesResponseType((int)HttpStatusCode.Redirect)]
  [ProducesResponseType(typeof(ApiResponse<ShortenedLink>), (int)HttpStatusCode.NotFound, "application/json")]
  [ProducesResponseType(typeof(ApiResponse<ShortenedLink>), (int)HttpStatusCode.BadRequest, "application/json")]
  [ProducesResponseType(typeof(ApiResponse<ShortenedLink>), (int)HttpStatusCode.UnavailableForLegalReasons, "application/json")]
  [ProducesResponseType(typeof(ApiResponse<ShortenedLink>), (int)HttpStatusCode.Gone, "application/json")]
  [ProducesResponseType(typeof(ApiResponse<ShortenedLink>), (int)HttpStatusCode.Unauthorized, "application/json")]
  [ProducesResponseType(typeof(ApiResponse<ShortenedLink>), (int)HttpStatusCode.ServiceUnavailable, "application/json")]
  public async Task<IActionResult> Get(string shortLinkId, [FromQuery] string? password) {
    var maybeShortLinkResult = await shortLinkRepository.GetAsync(shortLinkId)
      .TapError(e => logger.LogError("Could not read Shortened Link with {Id}, because {Error}", shortLinkId, e));

    if (maybeShortLinkResult.IsFailure) {
      return WantsHtml(HttpContext.Request)
        ? Redirect("/states/500")
        : ApiResponse<ShortenedLink>.Create(HttpStatusCode.InternalServerError, null, "Problem querying shortlink").ToResult();
    }

    maybeShortLinkResult.Value
      .TapSome(_ => metrics.Ok(shortLinkId))
      .TapNone(() => metrics.NotFound(shortLinkId));

    if (maybeShortLinkResult.Value.HasNoValue) {
      return WantsHtml(HttpContext.Request)
        ? Redirect("/states/404")
        : ApiResponse<ShortenedLink>.Create(HttpStatusCode.NotFound, null, "Shortlink not found").ToResult();
    }

    return await GetActionResultForExistingShortenedLinkRequest(maybeShortLinkResult.Value.Value, password);
  }

  /// <summary>
  ///   Create a new flinkified shortlink
  /// </summary>
  /// <param name="shortLinkRequest">information about the new shortlink</param>
  /// <returns>the shortened link</returns>
  [HttpPost("/links", Name = "CreateNewShortLink")]
  [ProducesResponseType(typeof(ApiResponse<ShortenedLink>), (int)HttpStatusCode.Created, "application/json")]
  [ProducesResponseType(typeof(ApiResponse<ShortenedLink>), (int)HttpStatusCode.BadRequest, "application/json")]
  [ProducesResponseType(typeof(ApiResponse<ShortenedLink>), (int)HttpStatusCode.UnavailableForLegalReasons, "application/json")]
  public async Task<IActionResult> Add(ShortLinkRequest shortLinkRequest) {
    var sanitizedId = shortLinkRequest.PreferredId?.Trim().ToLower();

    if (sanitizedId != null && SpecialShortLinkIds.ReservedShortLinkIds.Contains(sanitizedId))
      return ApiResponse<ShortenedLink>
        .Create(HttpStatusCode.UnavailableForLegalReasons, null, "Flink ID is reserved")
        .ToResult();


    var isBlocked = await blockedHostRepository.Contains(new Uri(shortLinkRequest.Uri).Host);
    if (isBlocked) {
      return ApiResponse<ShortenedLink>
        .Create(HttpStatusCode.UnavailableForLegalReasons, null, "The given URL is considered spam/malicious")
          .ToResult();
    }

    var actualId = shortLinkRequest.PreferredId?.Trim() ?? ShortLinkIdGenerator.CreateRandomId(shortLinkRequest.PreferredIdLength);
    var existCheckResult = await shortLinkRepository.GetAsync(actualId);

    if (existCheckResult.IsFailure) {
      return ApiResponse<ShortenedLink>
        .Create(HttpStatusCode.InternalServerError, null, "Could not check whether link exists already")
        .ToResult();
    }
    
    if(existCheckResult.Value.HasValue) {
      metrics.AlreadyExists(actualId);
      return ApiResponse<ShortenedLink>.Create(HttpStatusCode.BadRequest, null, "The Shortlink already exists").ToResult();
    }
    
    var shortenedLink = new ShortenedLink {
      Id = actualId,
      Creator = shortLinkRequest.Creator,
      Uri = shortLinkRequest.Uri,
      CreationDate = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
      ActiveFrom = shortLinkRequest.ActiveFrom?.ToUnixTimeSeconds() ?? 0,
      ActiveUntil = shortLinkRequest.ActiveUntil?.ToUnixTimeSeconds() ?? long.MaxValue,
      PasswordHash = shortLinkRequest.Password == null ? null : PasswordHasher.CalculatePasswordHash(shortLinkRequest.Password, actualId)
    };

    return await shortLinkRepository .AddAsync(shortenedLink)
      .TapError(e => logger.LogError("Could not create Shortened Link with {Id}, because {Error}", shortenedLink.Id, e))
      .Tap(shortLink => logger.LogInformation("Saved Shortened Link with {Id}", shortLink.Id))
      .Check(_ => scrapingQueue.EnqueueForScraping(shortenedLink))
      .Check(shortLink => PublishCreationNotification(shortLink, shortenedLink))
      .Tap(_ => metrics.Created())
      .TapError(_ => metrics.AlreadyExists(shortenedLink.Id))
      .Match(
        s => ApiResponse<ShortenedLink>.Create(HttpStatusCode.Created, s.StripPassword(), "Link shortened", $"/{s.Id}").ToResult(),
        _ => ApiResponse<ShortenedLink>.Create(HttpStatusCode.BadRequest, null, "Failed to add Shortlink").ToResult());
  }

  private Result PublishCreationNotification(ShortenedLink shortLink, ShortenedLink shortenedLink) {
    var request = HttpContext.Request;
    var shortUrl = $"{request.Scheme}://{request.Headers.Host}/links/{shortenedLink.Id}";
    var embedUrl = $"{request.Scheme}://{request.Headers.Host}/links/{shortenedLink.Id}/embed";
    var isPasswordProtected = shortenedLink.PasswordHash is null ? "no" : "yes";

    return eventNotificationQueue.Publish(new ShortLinkCreatedEvent {
      Emoji = "🆕",
      Title = $"Link shortened",
      Message =
        $"Link with Id {shortLink.Id}, targeting {shortLink.Uri} was created by '{shortLink.Creator}' (Password-protected: {isPasswordProtected})",
      ImageUrl = HttpContext.Request.GetThumbnailUrl(shortLink.Id),
      Urls = new Dictionary<string, Uri> {
        { "Long Link", new Uri(shortenedLink.Uri) },
        { "Short URL", new Uri(shortUrl) },
        { "Embed", new Uri(embedUrl) }
      },
    });
  }

  /// <summary>
  ///   Delete a (previously flinkified) shortlink
  /// </summary>
  /// <param name="shortLinkId">Id of the shortlink to remove</param>
  [HttpDelete("/links/{shortLinkId}", Name = "RemoveShortLink")]
  [ProducesResponseType((int)HttpStatusCode.OK)]
  [ProducesResponseType((int)HttpStatusCode.InternalServerError)]
  public async Task<IActionResult> Delete(string shortLinkId) {
    var maybeShortLinkResult = await shortLinkRepository.GetAsync(shortLinkId);
    if (maybeShortLinkResult.IsFailure) {
      return ApiResponse<ShortenedLink>
        .Create(HttpStatusCode.InternalServerError, null, "Could not check whether link exists already")
        .ToResult();
    }

    if (maybeShortLinkResult.Value.HasNoValue) {
      return ApiResponse<ShortenedLink>
        .Create(HttpStatusCode.OK, null, "Shortlink does not exist")
        .ToResult();
    }

    var shortLink = maybeShortLinkResult.Value.Value;

    return await shortLinkRepository.DeleteAsync(shortLinkId)
      .Tap(() => logger.LogInformation("Deleted Shortened Link {Id}", shortLinkId))
      .Tap(() => PublishDeletionNotification(shortLink))
      .TapError(f => logger.LogError("Could not delete Shortened Link {Id}, because {Error}", shortLinkId, f))
      .Match(
        () => ApiResponse<ShortenedLink>
          .Create(HttpStatusCode.OK, shortLink.StripPassword(), "Shortlink does not exist")
          .ToResult(), 
        f => ApiResponse<ShortenedLink>
          .Create(HttpStatusCode.InternalServerError, shortLink.StripPassword(), "Could not delete Shortened Link")
          .ToResult());
    
  }

  private void PublishDeletionNotification(ShortenedLink shortLink) {
    eventNotificationQueue.Publish(new ShortLinkCreatedEvent {
      Emoji = "🆕",
      Title = "Link deleted",
      Message = $"Link with Id {shortLink.Id}, targeting {shortLink.Uri} was deleted'",
      ImageUrl = HttpContext.Request.GetThumbnailUrl(shortLink.Id),
      Urls = new Dictionary<string, Uri> {
        { "Long Link", new Uri(shortLink.Uri) }
      },
    });
  }

  private async Task<IActionResult> GetActionResultForExistingShortenedLinkRequest(ShortenedLink shortenedLink, string? clientProvidedPassword) {
    var now = timeProvider.GetUtcNow().ToUnixTimeSeconds();
    var uri = new Uri(shortenedLink.Uri).Host.ToLowerInvariant();

    if (await blockedHostRepository.Contains(uri)) {
      // Host is on a Spam Protection Blocklist
      return WantsHtml(HttpContext.Request)
        ? Redirect($"/states/451")
        : ApiResponse<ShortenedLink>.Create(HttpStatusCode.UnavailableForLegalReasons, shortenedLink.StripPassword(), "Host is blocked").ToResult();
    }

    if (now < shortenedLink.ActiveFrom) {
      // The Shortened Link is not yet active but will be later
      return WantsHtml(HttpContext.Request)
        ? Redirect($"/cd/{shortenedLink.Id}?activeFrom={shortenedLink.ActiveFrom}")
        : ApiResponse<ShortenedLink>.Create(HttpStatusCode.ServiceUnavailable, shortenedLink.StripPassword().StripUri(), "Link not yet active").ToResult();
    }

    if (shortenedLink.PasswordHash != null && clientProvidedPassword == null) {
      // No password provided (even though a password is needed)
      return WantsHtml(HttpContext.Request)
        ? Redirect($"/pw/{shortenedLink.Id}")
        : ApiResponse<ShortenedLink>.Create(HttpStatusCode.Unauthorized, null, "Password required").ToResult();
    }

    if (now >= shortenedLink.ActiveUntil) {
      // The shortened Link is expired and not yet valid anymore
      return WantsHtml(HttpContext.Request)
        ? Redirect("/states/410")
        : ApiResponse<ShortenedLink>.Create(HttpStatusCode.Gone, shortenedLink.StripPassword().StripUri(), "Link expired already").ToResult();
    }

    // Short Link is password protected and provided password does not match
    if (shortenedLink.PasswordHash != null
        && clientProvidedPassword != null
        && !PasswordHasher.VerifyPasswordHash(shortenedLink.PasswordHash, clientProvidedPassword, shortenedLink.Id)) {
      return WantsHtml(HttpContext.Request)
        ? Redirect("/states/401")
        : ApiResponse<ShortenedLink>.Create(HttpStatusCode.Unauthorized, null, "Wrong Password provided").ToResult();
    }

    return Redirect(shortenedLink.Uri);
  }

  private static bool WantsHtml(HttpRequest request) {
    var accept = request.Headers.Accept.ToString();
    return !string.IsNullOrWhiteSpace(accept) && accept.Contains("text/html", StringComparison.OrdinalIgnoreCase);
  }
}
