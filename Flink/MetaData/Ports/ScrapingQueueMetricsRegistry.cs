using System.Diagnostics.Metrics;

namespace Flink.MetaData.Ports;

public class ScrapingQueueMetricsRegistry {
  public const string MeterName = "scrapingqueue";
  private const string Unit = "requests";
  private readonly Counter<long> _queueSize;


  public ScrapingQueueMetricsRegistry(IMeterFactory meterFactory) {
    var meter = meterFactory.Create(MeterName, "1.0.0");
    _queueSize = meter
      .CreateCounter<long>("queue_size", Unit, "Current amount of elements in the Scraping Queue");
  }

  public void Enqueue() {
    _queueSize.Add(1);
  }

  public void Dequeue() {
    _queueSize.Add(-1);
  }
}
