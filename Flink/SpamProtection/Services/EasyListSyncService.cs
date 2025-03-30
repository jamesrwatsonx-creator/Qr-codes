using CSharpFunctionalExtensions;
using Flink.SpamProtection.Ports.BusinessLogic;
using Flink.SpamProtection.Ports.Models;
using Flink.SpamProtection.Ports.Repositories;
using Microsoft.Extensions.Options;

namespace Flink.SpamProtection.Services;

public class EasyListSyncService(
  ILogger<EasyListSyncService> logger,
  HttpClient httpClient,
  IOptions<SpamProtectionConfiguration> spamProtectionOptions,
  IForbiddenDomainRepository repo) : BackgroundService {
  
  private async Task<string> FetchEasyList(string url) {
    var r = await httpClient.GetAsync(url);
    var body = await r.Content.ReadAsStringAsync();
    return body;
  }
  
  protected override async Task ExecuteAsync(CancellationToken cancellationToken) {
    if (!spamProtectionOptions.Value.Enabled)
      return;
    
    repo.AddForbiddenDomains(spamProtectionOptions.Value.CustomBlockedDomains);

    while (!cancellationToken.IsCancellationRequested) {
      foreach (var src in spamProtectionOptions.Value.BlockLists) {
        logger.LogInformation("Getting Block List {BlockListSrc}", src);
        try {
          var easyListPayload = await FetchEasyList(src);
          EasyListParser.ParseHeader(src, easyListPayload).Tap(repo.AddDefinition);
          repo.AddForbiddenDomains(EasyListParser.ParseForbiddenDomains(easyListPayload));
        } catch (Exception exc) {
          logger.LogError("Failed to synchronize {BlockListSrc} because {Error}", src, exc);
        }
      }

      await Task.Delay(TimeSpan.FromHours(spamProtectionOptions.Value.RefreshIntervalInHours), cancellationToken);
    }
  }
}
