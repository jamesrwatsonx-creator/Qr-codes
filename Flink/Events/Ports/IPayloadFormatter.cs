using Flink.Events.Ports.Models;

namespace Flink.Events.Ports;

public interface IPayloadFormatter {
  public string Format(EventNotification ev, EventNotificationSinkConfiguration configuration);
}
