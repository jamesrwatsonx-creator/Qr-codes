using System.Collections.Concurrent;
using Flink.SpamProtection.Models;
using Flink.SpamProtection.Ports.Repositories;

namespace Flink.SpamProtection.Repositories;

public class ForbiddenDomainRepository : IForbiddenDomainRepository {
  private readonly ConcurrentBag<string> _forbiddenDomains = [];
  private readonly ConcurrentBag<EasyListSource> _sources = [];

  public EasyListSource[] UsedEasyListSources {
    get => _sources.ToArray();
  }

  public int BlockedDomainCount {
    get => _forbiddenDomains.Count;
  }

  public void AddForbiddenDomains(IEnumerable<string> domains) {
    domains.ToList().ForEach(AddForbiddenDomain);
  }

  public bool ContainsDomain(string domain) {
    var sanitizedDomain = domain.Trim().ToLower();
    return _forbiddenDomains.Any(sanitizedDomain.EndsWith);
  }

  public void AddDefinition(EasyListSource source) {
    if (_sources.Where(x => x.Source == source.Source).Any(x => x.Version >= source.Version))
      return;

    _sources.Add(source);
  }

  private void AddForbiddenDomain(string domain) {
    var sanitizedDomain = domain.Trim().ToLower();
    if (_forbiddenDomains.Contains(sanitizedDomain))
      return;
    _forbiddenDomains.Add(sanitizedDomain);
  }
}
