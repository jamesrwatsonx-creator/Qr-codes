using Flink.SpamProtection.Models;
using Flink.SpamProtection.Ports.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Flink.SpamProtection.Ports.Controllers;

public class SpamProtectionStatistics {
  public int BlockedDomainsCount { get; set; }
  public int SpamProtectionSources { get; set; }
}

[ApiController]
public class SpamProtectionController(IForbiddenDomainRepository forbiddenDomainRepository) : ControllerBase {
  
  [HttpGet("/spam-protection")]
  public SpamProtectionStatistics GetStatistics() {
    return new SpamProtectionStatistics() {
      BlockedDomainsCount = forbiddenDomainRepository.BlockedDomainCount,
      SpamProtectionSources = forbiddenDomainRepository.UsedEasyListSources.Length
    };
  }
  
  [HttpGet("/spam-protection/sources")]
  public EasyListSource[] GetSources() {
    return forbiddenDomainRepository.UsedEasyListSources;
  }
  
}
