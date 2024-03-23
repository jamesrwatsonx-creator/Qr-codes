using System.Diagnostics.Metrics;
using Flink.Shared;

namespace Flink.ShortLinks.Ports;

public class ShortLinkMetricsRegistry {
  public const string MeterName = "shortlinks";
  private const string Unit = "requests";
  private readonly Counter<long> _okShortLinkGets;
  private readonly Counter<long> _notFoundShortLinkGets;
  private readonly Counter<long> _totalShortLinkGets;
  private readonly Counter<long> _createShortLinkPosts;
  private readonly Counter<long> _alreadyExistsShortLinkPosts;
  private readonly Counter<long> _totalShortLinkPosts;
  
  public ShortLinkMetricsRegistry(IMeterFactory meterFactory) {
    var meter = meterFactory.Create(MeterName, "1.0.0");
    _okShortLinkGets = meter
      .CreateCounter<long>("ok_get", Unit, "number of successful (HTTP 200) HTTP GET requests /to/{id}");
    _notFoundShortLinkGets =  meter
      .CreateCounter<long>("notfound_get", Unit, "number of not found (HTTP 404) HTTP GET requests /to/{id}");
    _totalShortLinkGets = meter
      .CreateCounter<long>("total_get", Unit, "total number of HTTP GET (successful, not found or failed) requests /to/{id}");
    
    _createShortLinkPosts = meter
      .CreateCounter<long>("create_post", Unit, "number of successful (HTTP 201) HTTP POST create requests /to");
    _alreadyExistsShortLinkPosts =  meter
      .CreateCounter<long>("alreadyexists_post", Unit, "number of successful (HTTP 400) HTTP POST create requests /to");
    _totalShortLinkPosts = meter
      .CreateCounter<long>("total_post", Unit, "total number of HTTP POST (successful, already exists or failed) requests /to/{id}");
  }

  public void Ok(string id) {
    _totalShortLinkGets.Add(1);
    _okShortLinkGets.Add(1, Tag.Create(nameof(id), id));
  }
  
  public void NotFound(string id) {
    _totalShortLinkGets.Add(1);
    _notFoundShortLinkGets.Add(1, Tag.Create(nameof(id), id));
  }

  public void Created() {
    _totalShortLinkPosts.Add(1);
    _createShortLinkPosts.Add(1);
  }

  public void AlreadyExists(string id) {
    _totalShortLinkPosts.Add(1);
    _alreadyExistsShortLinkPosts.Add(1, Tag.Create(nameof(id), id));
  }
}
