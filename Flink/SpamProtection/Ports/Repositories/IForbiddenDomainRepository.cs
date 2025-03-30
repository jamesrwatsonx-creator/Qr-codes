using Flink.SpamProtection.Models;

namespace Flink.SpamProtection.Ports.Repositories;

public interface IForbiddenDomainRepository {
  public EasyListSource[] UsedEasyListSources { get;  }
  public int BlockedDomainCount { get; }
  
  public void AddForbiddenDomains(IEnumerable<string> domains);
  public bool ContainsDomain(string domain);

  public void AddDefinition(EasyListSource source);
}
