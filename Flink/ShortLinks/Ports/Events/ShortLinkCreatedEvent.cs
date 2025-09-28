using Flink.Events.Ports;

namespace Flink.ShortLinks.Ports.Events;

public class ShortLinkCreatedEventNotification() : EventNotification(FlinkEventNotificationType.FlinkLinkShortened) {
}
