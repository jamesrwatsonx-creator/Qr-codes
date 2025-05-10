using System.Diagnostics.Metrics;
using Flink.Shared;

namespace Flink.Embed.Ports;

public class EmbedMetricsRegistry {
  public const string MeterName = "embeds";
  private const string Unit = "requests";
  private readonly Counter<long> _embedAllViews;
  private readonly Counter<long> _embedOkViews;

  public EmbedMetricsRegistry(IMeterFactory meterFactory) {
    var meter = meterFactory.Create(MeterName, "1.0.0");
    _embedOkViews = meter
      .CreateCounter<long>("ok_get", Unit, "number of successful (HTTP 200) HTTP GET requests for a specific embed");
    _embedAllViews = meter
      .CreateCounter<long>("total_get", Unit, "total number of successful (HTTP 200) HTTP GET requests for embeds");
  }

  public void Success(string id) {
    _embedAllViews.Add(1);
    _embedOkViews.Add(1, Tag.Create(nameof(id), id));
  }
}
