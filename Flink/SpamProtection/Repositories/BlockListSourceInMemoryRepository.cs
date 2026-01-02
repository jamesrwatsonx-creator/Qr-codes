using System.Collections.Concurrent;
using Flink.SpamProtection.Models;
using Flink.SpamProtection.Ports.Repositories;

namespace Flink.SpamProtection.Repositories;

public class BlockListSourceInMemoryRepository : IBlockListSourceRepository {
  private readonly ConcurrentBag<BlockListSource> _sources = new();

  public Task<bool> IsBlockListAlreadyKnownAsync(BlockListSource blockListSource) {
    return Task.FromResult(_sources.Any(s => s.Sha3 == blockListSource.Sha3 && s.Source == blockListSource.Source));
  }
  
  public async Task RegisterBlockListUpdateAsync(BlockListSource blockListSource) {
    var isSourceKnown = await IsBlockListAlreadyKnownAsync(blockListSource);
    if(!isSourceKnown)
      _sources.Add(blockListSource);
  }

  public Task<IEnumerable<BlockListSource>> GetBlockListSourcesAsync() => Task.FromResult<IEnumerable<BlockListSource>>(_sources.ToList());
}
