using CSharpFunctionalExtensions;

namespace Flink.Events.Ports;

public interface IEventNotificationQueue {
  public Result Publish<T>(T ev) where T : EventNotification;
  public IEnumerable<T> Dequeue<T>(int dequeueCount) where T : EventNotification;
}
