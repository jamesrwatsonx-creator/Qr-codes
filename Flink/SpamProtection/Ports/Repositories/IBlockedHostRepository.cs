namespace Flink.SpamProtection.Ports.Repositories;

public interface IBlockedHostRepository {
  public Task<int> CountAsync();
  public Task InsertAsync(IEnumerable<string> hosts);
  public Task<bool> Contains(string host);
}
