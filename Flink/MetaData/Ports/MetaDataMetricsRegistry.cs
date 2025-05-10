using System.Diagnostics.Metrics;
using Flink.Shared;

namespace Flink.MetaData.Ports;

// ReSharper disable once ClassNeverInstantiated.Global
public class MetaDataMetricsRegistry {
  public const string MeterName = "meta";
  private const string Unit = "requests";
  private readonly Counter<long> _metaAllRequests;
  private readonly Counter<long> _metaOkRequests;

  public MetaDataMetricsRegistry(IMeterFactory meterFactory) {
    var meter = meterFactory.Create(MeterName, "1.0.0");
    _metaOkRequests = meter
      .CreateCounter<long>("ok_get", Unit, "number of successful (HTTP 200) HTTP GET requests for specific meta data of a shortlink");
    _metaAllRequests = meter
      .CreateCounter<long>("total_get", Unit, "total number of successful (HTTP 200) HTTP GET requests for meta data");
  }

  public void Success(string id) {
    _metaAllRequests.Add(1);
    _metaOkRequests.Add(1, Tag.Create(nameof(id), id));
  }
}
