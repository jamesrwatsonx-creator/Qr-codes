using CSharpFunctionalExtensions;
using Flink.Events.Ports;

namespace Flink.Events;

public class InMemoryEventNotificationNotificationQueue : IEventNotificationQueue {
  private readonly Queue<EventNotification> _queue = new(1024);

  public Result Publish<T>(T ev) where T : EventNotification {
    _queue.Enqueue(ev);
    return Result.Success();
  }

  public IEnumerable<T> Dequeue<T>(int dequeueCount) where T : EventNotification {
    return Enumerable.Range(0, Math.Min(dequeueCount, _queue.Count))
      .Select(_ => TryDequeueSingle())
      .Where(x => x.HasValue)
      .Select(x => x.Value)
      .Cast<T>();
  }

  private Maybe<EventNotification> TryDequeueSingle() {
    try {
      return Maybe.From(_queue.Dequeue());
    } catch {
      return Maybe.None;
    }
  }
}
