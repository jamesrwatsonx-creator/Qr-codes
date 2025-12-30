using Flink.Events.Ports;
using Flink.Events.Ports.Events;

namespace Flink.Events.Services;

public class FlinkBootEventBackgroundService(IEventNotificationQueue notificationQueue) : BackgroundService {
  protected override Task ExecuteAsync(CancellationToken stoppingToken) {
    notificationQueue.Publish(new FlinkBootedEvent {
      Title = "Flink started",
      Emoji = "🚀",
      Message = $"Flink on Host {Environment.MachineName} started successfully. Ready to shorten URLs",
      Urls = new Dictionary<string, Uri>(),
      ImageUrl = null
    });
    
    return Task.CompletedTask;
  }
}
