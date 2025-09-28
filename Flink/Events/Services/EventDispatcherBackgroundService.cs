using System.Text;
using CSharpFunctionalExtensions;
using Flink.Events.PayloadFormatters;
using Flink.Events.Ports;
using Flink.Events.Ports.Models;
using Microsoft.Extensions.Options;

namespace Flink.Events.Services;

public class EventDispatcherBackgroundService(IEventNotificationQueue eventNotificationQueue, HttpClient httpClient, IOptions<EventNotificationConfiguration> config)
  : BackgroundService {
  private static readonly NtfyFormatter NtfyFormatter = new();
  private static readonly DiscordFormatter DiscordFormatter = new();
  private static readonly MattermostSlackRocketChatPayloadFormatter MattermostSlackRocketChatPayloadFormatter = new();

  protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
    while (!stoppingToken.IsCancellationRequested) {
      await Task.WhenAll(eventNotificationQueue
        .Dequeue<EventNotification>(100)
        .Select(SendSingleAsync)
        .ToArray());
      await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
    }
  }

  private async Task<Result> SendSingleAsync(EventNotification ev) {
    try {
      foreach (var sink in config.Value.Sinks) {
        if(!sink.Events.Contains(ev.EventType))
          continue;
        
        var payload = sink.PayloadFormat.ToLower() switch {
          "ntfy" => NtfyFormatter.Format(ev, sink),
          "discord" => DiscordFormatter.Format(ev, sink),
          "mattermost" => MattermostSlackRocketChatPayloadFormatter.Format(ev, sink),
          "slack" => MattermostSlackRocketChatPayloadFormatter.Format(ev, sink),
          "rocketchat" => MattermostSlackRocketChatPayloadFormatter.Format(ev, sink),
          _ => throw new NotImplementedException()
        };

        var uri = new Uri(sink.Url);
        var url = sink.PayloadFormat.ToLower() switch {
          "ntfy" => $"{uri.Scheme}://{uri.Host}:{uri.Port}/",
          _ => sink.Url
        };

        var requestPayload = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await (sink.Method.ToUpper().Trim() switch {
          "POST" => httpClient.PostAsync(url, requestPayload),
          "PUT" => httpClient.PutAsync(url, requestPayload),
          "DELETE" => httpClient.DeleteAsync(url),
          "GET" => httpClient.GetAsync(url),
          _ => throw new NotImplementedException()
        });
      }
    } catch (Exception e) {
      return Result.Failure(e.Message);
    }

    return Result.Success();
  }
}
