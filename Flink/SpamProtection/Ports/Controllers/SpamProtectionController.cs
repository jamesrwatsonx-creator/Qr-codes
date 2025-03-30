using System.Net;
using Flink.SpamProtection.Models;
using Flink.SpamProtection.Ports.Models;
using Flink.SpamProtection.Ports.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Flink.SpamProtection.Ports.Controllers;

[ApiController]
[Produces("application/json")]
public class SpamProtectionController(IForbiddenDomainRepository forbiddenDomainRepository) : ControllerBase {
  
  /// <summary>
  /// Receive statistics about spam protection
  /// </summary>
  /// <returns></returns>
  [HttpGet("/spam-protection")]
  [ProducesResponseType((int)HttpStatusCode.OK)]
  public SpamProtectionStatistics GetStatistics() {
    return new SpamProtectionStatistics() {
      BlockedDomainsCount = forbiddenDomainRepository.BlockedDomainCount,
      SpamProtectionSources = forbiddenDomainRepository.UsedEasyListSources.Length
    };
  }
  
  /// <summary>
  /// Return sources used for spam protection
  /// </summary>
  /// <returns></returns>
  [HttpGet("/spam-protection/sources")]
  [ProducesResponseType((int)HttpStatusCode.OK)]
  public EasyListSource[] GetSources() {
    return forbiddenDomainRepository.UsedEasyListSources;
  }
  
}
