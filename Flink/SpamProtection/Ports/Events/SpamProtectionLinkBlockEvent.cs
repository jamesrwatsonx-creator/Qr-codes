using Flink.Events.Ports;

namespace Flink.SpamProtection.Ports.Events;

public class SpamProtectionLinkBlockEvent() : EventNotification(FlinkEventNotificationType.FlinkLinkBlocked) {
  
}
