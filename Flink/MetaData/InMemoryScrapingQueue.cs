using CSharpFunctionalExtensions;
using Flink.Domain;
using Flink.MetaData.Ports;
using Flink.Shared.Extensions;

namespace Flink.MetaData;

public class InMemoryScrapingQueue(ScrapingQueueMetricsRegistry scrapingQueueMetricsRegistry) : IScrapingQueue {
  private readonly Queue<ShortenedLink> _taskQueue = new(1024);
  
  public Result EnqueueForScraping(ShortenedLink shortenedLink) {
    _taskQueue.Enqueue(shortenedLink);
    scrapingQueueMetricsRegistry.Enqueue();
    return Result.Success();
  }

  public IEnumerable<ShortenedLink> Dequeue(int dequeueCount) {
    return Enumerable.Range(0, Math.Min(dequeueCount, _taskQueue.Count))
      .Select(_ => TryDequeueSingle())
      .Where(maybeShortenedLink => maybeShortenedLink.HasValue)
      .Select(maybeShortenedLink => maybeShortenedLink.Value);
  }

  private Maybe<ShortenedLink> TryDequeueSingle() {
    try {
      return Maybe.From(_taskQueue.Dequeue())
        .TapSome(_ => scrapingQueueMetricsRegistry.Dequeue());
    } catch {
      return Maybe.None;
    }
  }
}
