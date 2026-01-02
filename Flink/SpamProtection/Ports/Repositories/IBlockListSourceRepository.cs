using Flink.SpamProtection.Models;

namespace Flink.SpamProtection.Ports.Repositories;

public interface IBlockListSourceRepository {
  public Task<bool> IsBlockListAlreadyKnownAsync(BlockListSource blockListSource);
  public Task RegisterBlockListUpdateAsync(BlockListSource blockListSource);
  public Task<IEnumerable<BlockListSource>> GetBlockListSourcesAsync();
}
