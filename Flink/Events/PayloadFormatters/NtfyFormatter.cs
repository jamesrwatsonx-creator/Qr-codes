using System.Text.Json;
using System.Text.Json.Serialization;
using Flink.Events.Ports;
using Flink.Events.Ports.Models;

namespace Flink.Events.PayloadFormatters;

public class NtfyFormatter : IPayloadFormatter {
  private static readonly JsonSerializerOptions SerializerOptions = new() {
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    WriteIndented = false
  };

  public string Format(EventNotification ev, EventNotificationSinkConfiguration configuration) {
    var payload = new Dictionary<string, object?> {
      ["topic"] = ExtractTopic(configuration),
      ["title"] = $"{ev.Emoji} {ev.Title}",
      ["message"] = ev.Message,
      ["delay"] = "10s",
      ["icon"] = "https://gitlab.com/rtraceio/web/flink/-/raw/main/Flink/wwwroot/static/img/flink-logo.png",
      ["actions"] = ev.Urls?
        .Select(x => new Dictionary<string, object?> {
          ["action"] = "view",
          ["label"] = x.Key,
          ["url"] = x.Value.ToString(),
          ["clear"] = false
        }),
    };
    
    return JsonSerializer.Serialize(payload, SerializerOptions);
  }

  private static string ExtractTopic(EventNotificationSinkConfiguration configuration) =>
    new Uri(configuration.Url).AbsolutePath.TrimStart('/');
}
