using System.Collections.Concurrent;
using CSharpFunctionalExtensions;
using Flink.Domain;
using Flink.MetaData.Ports;

namespace Flink.MetaData;

public class InMemoryScrapingQueue : IScrapingQueue {
  private readonly Queue<ShortenedLink> _taskQueue = new(1024);
  
  public Result EnqueueForScraping(ShortenedLink shortenedLink) {
    _taskQueue.Enqueue(shortenedLink);
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
      return Maybe.From(_taskQueue.Dequeue());
    } catch {
      return Maybe.None;
    }
  }
}
