using Flink.Shared;
using Flink.SpamProtection.Ports.Repositories;

namespace Flink.SpamProtection.Repositories;

public sealed class BlockedHostInMemoryRepository : IBlockedHostRepository {
  private const int HashsetCapacity = 1000000;
  private readonly ConcurrentHashSet<string> _forbiddenHostnamesExactMatch = new(HashsetCapacity, StringComparer.OrdinalIgnoreCase);

  public Task<int> CountAsync() {
    return Task.FromResult(_forbiddenHostnamesExactMatch.Count());
  }

  public Task InsertAsync(IEnumerable<string> hosts) {
    _forbiddenHostnamesExactMatch.Insert(hosts);
    return Task.CompletedTask;
  }

  Task<bool> IBlockedHostRepository.Contains(string host) {
    return Task.FromResult(_forbiddenHostnamesExactMatch.Contains(host));
  }
}
