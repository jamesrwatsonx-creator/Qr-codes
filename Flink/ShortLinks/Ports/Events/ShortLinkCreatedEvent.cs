using Flink.Events.Ports;

namespace Flink.ShortLinks.Ports.Events;

public class ShortLinkCreatedEvent() : EventNotification(FlinkEventNotificationType.FlinkLinkShortened) {
}
