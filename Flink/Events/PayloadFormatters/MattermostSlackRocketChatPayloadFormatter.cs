using System.Text.Json;
using System.Text.Json.Serialization;
using Flink.Events.Ports;
using Flink.Events.Ports.Models;

public class MattermostSlackRocketChatPayloadFormatter : IPayloadFormatter {
  private static readonly JsonSerializerOptions SerializerOptions = new() {
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    WriteIndented = false
  };

  
  public string Format(EventNotification ev, EventNotificationSinkConfiguration configuration) {
    var baseText = ev.Message;

    if (ev.Urls?.Count > 0) {
      var links = string.Join("\n", ev.Urls.Select(kvp => $"*{kvp.Key}:* {kvp.Value}"));
      baseText = $"{baseText}\n\n{links}";
    }

    var attachment = new Dictionary<string, object?> {
      ["title"] = ev.Title,
      ["text"] = baseText,
      ["mrkdwn_in"] = new[] { "text", "pretext" }
    };
    
    if (!string.IsNullOrWhiteSpace(ev.Emoji)) {
      attachment["pretext"] = ev.Emoji;
    }

    if (!string.IsNullOrWhiteSpace(ev.ImageUrl)) {
      attachment["image_url"] = ev.ImageUrl;
    }
    var payload = new Dictionary<string, object?> {
      ["attachments"] = new[] { attachment }
    };

    return JsonSerializer.Serialize(payload, SerializerOptions);
  }
}
