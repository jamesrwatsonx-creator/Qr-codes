using Flink.Events.Ports;

namespace Flink.ShortLinks.Ports.Events;

public class ShortLinkDeletedEvent() : EventNotification(FlinkEventNotificationType.FlinkLinkDeleted) {
}
