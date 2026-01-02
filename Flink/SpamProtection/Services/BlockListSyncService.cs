using Flink.SpamProtection.Ports.BusinessLogic;
using Flink.SpamProtection.Ports.Repositories;
using Flink.SpamProtection.Ports.Models;
using Microsoft.Extensions.Options;

namespace Flink.SpamProtection.Services;

public class BlockListSyncService(
  ILogger<BlockListSyncService> logger,
  HttpClient httpClient,
  IOptions<SpamProtectionConfiguration> spamProtectionOptions,
  IBlockedHostRepository blockedHostRepository,
  IBlockListSourceRepository blockListSourceRepository) : BackgroundService {

  protected override async Task ExecuteAsync(CancellationToken cancellationToken) {
    if (!spamProtectionOptions.Value.Enabled)
      return;

    await blockedHostRepository.InsertAsync(spamProtectionOptions.Value.CustomBlockedHosts);

    while (!cancellationToken.IsCancellationRequested) {
      foreach (var src in spamProtectionOptions.Value.BlockListSources) {
        logger.LogInformation("Updating SpamProtection BlockList from {BlockListSrc}", src);
        try {
          var blockListText = await FetchBlockList(src);
          var blockListSource = BlockListParser.ParseBlockListSourceInformation(src, blockListText);
          if (blockListSource == null) {
            logger.LogWarning("Could not parse BlockList Header for {BlockListSrc}", src);
            continue;
          }
           
          var isBlockListSourceKnownAlready = await blockListSourceRepository.IsBlockListAlreadyKnownAsync(blockListSource);
          if (isBlockListSourceKnownAlready) {
            logger.LogDebug("BlockList {BlockListSrc} did not change, nothing to do", src);
            continue;
          }
          
          await blockedHostRepository.InsertAsync(BlockListParser.ParseExactMatches(blockListText));
          await blockListSourceRepository.RegisterBlockListUpdateAsync(blockListSource);
        } catch (Exception exc) {
          logger.LogError("Failed to synchronize {BlockListSrc} because {Error}", src, exc);
        }
      }

      await Task.Delay(TimeSpan.FromHours(spamProtectionOptions.Value.RefreshIntervalInHours), cancellationToken);
    }
  }
  
  private async Task<string> FetchBlockList(string url) {
    var httpResponse = await httpClient.GetAsync(url);
    return await httpResponse.Content.ReadAsStringAsync();
  }
}
