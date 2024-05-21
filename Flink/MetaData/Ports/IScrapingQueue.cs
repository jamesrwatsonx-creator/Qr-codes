using CSharpFunctionalExtensions;
using Flink.Domain;

namespace Flink.MetaData.Ports;

public interface IScrapingQueue {
  public Result EnqueueForScraping(ShortenedLink shortenedLink);
  public IEnumerable<ShortenedLink> Dequeue(int dequeueCount);
}
