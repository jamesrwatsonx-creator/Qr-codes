using System.Net;
using Flink.Shared.ApiResponse;
using Flink.Shared.Extensions;
using Flink.SpamProtection.Models;
using Flink.SpamProtection.Ports.Models;
using Flink.SpamProtection.Ports.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Flink.SpamProtection.Ports.Controllers;

[ApiController]
[Produces("application/json")]
public class SpamProtectionController(
  IOptions<SpamProtectionConfiguration> spamProtectionConfiguration,
  IBlockedHostRepository blockedHostRepository,
  IBlockListSourceRepository blockListSourceRepository) : ControllerBase {
  /// <summary>
  ///   Receive spam protection status
  /// </summary>
  /// <returns></returns>
  [HttpGet("/spam-protection")]
  [ProducesResponseType((int)HttpStatusCode.OK)]
  public async Task<IActionResult> GetStatistics() {
    var blockedHostsCount = await blockedHostRepository.CountAsync();
    var blockListSources = await blockListSourceRepository.GetBlockListSourcesAsync();
    
    return ApiResponse<SpamProtectionStatus>.Create(HttpStatusCode.OK, new SpamProtectionStatus {
      Enabled = spamProtectionConfiguration.Value.Enabled,
      BlockedHostsCount = blockedHostsCount,
      BlockListSourcesCount = blockListSources.Count(),
    }).ToResult();
  }

  /// <summary>
  ///   Return sources of Block Lists used for spam protection
  /// </summary>
  /// <returns></returns>
  [HttpGet("/spam-protection/sources")]
  [ProducesResponseType((int)HttpStatusCode.OK)]
  public async Task<BlockListSource[]> GetSources() {
    var blockListSources = await blockListSourceRepository.GetBlockListSourcesAsync();
    return blockListSources.ToArray();
  }

  /// <summary>
  ///   Check whether any given Host/FQDN IPv4/IPv6 address is on a Spam Protection BlockList
  /// </summary>
  /// <param name="host">Host, FQDN, IPv4, IPv6</param>
  [HttpGet("/spam-protection/blocks")]
  [ProducesResponseType(typeof(ApiResponse<IEnumerable<HostBlockStatus>>), (int)HttpStatusCode.OK)]
  [ProducesResponseType(typeof(ApiResponse<>), (int)HttpStatusCode.BadRequest)]
  public async Task<IActionResult> CheckHostBlocks([FromQuery]string[]? host) {
    if (host == null || host.Length == 0) {
      return ApiResponse<HostBlockStatus>.Create(HttpStatusCode.BadRequest, null, "No Host(s) provided").ToResult();
    }

    try {
      var d = await host
        .Where(h => !string.IsNullOrWhiteSpace(h))
        .ToDictionary(k => k, v => blockedHostRepository.Contains(v.ToLowerInvariant()))
        .AwaitValueTasks();
      
      return ApiResponse<IEnumerable<HostBlockStatus>>
        .Create(HttpStatusCode.OK, d.Select(kv => new HostBlockStatus { Blocked = kv.Value, Host = kv.Key }))
        .ToResult();
    } catch {
      return ApiResponse<object> .Create(HttpStatusCode.InternalServerError, null).ToResult();
    }
  }
}
