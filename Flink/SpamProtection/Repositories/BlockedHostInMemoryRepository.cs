using Flink.Shared;
using Flink.Shared.DataStructures;
using Flink.SpamProtection.Ports.Repositories;

namespace Flink.SpamProtection.Repositories;

public sealed class BlockedHostInMemoryRepository : IBlockedHostRepository {
  public const int HashsetCapacity = 1000000;
  private readonly ConcurrentHashSet<string> _forbiddenHosts = new(HashsetCapacity, StringComparer.OrdinalIgnoreCase);

  public Task<int> CountAsync() {
    return Task.FromResult(_forbiddenHosts.Count());
  }

  public Task InsertAsync(IEnumerable<string> hosts) {
    _forbiddenHosts.Insert(hosts);
    return Task.CompletedTask;
  }

  public Task<bool> Contains(string host) {
    return Task.FromResult(_forbiddenHosts.Contains(host));
  }
}
