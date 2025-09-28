using System.Text.Json;
using System.Text.Json.Serialization;
using Flink.Events.Ports;
using Flink.Events.Ports.Models;

namespace Flink.Events.PayloadFormatters;

public class DiscordFormatter : IPayloadFormatter {
  private static readonly JsonSerializerOptions SerializerOptions = new() {
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    WriteIndented = false
  };

  public string Format(EventNotification ev, EventNotificationSinkConfiguration configuration) {
    var links = ev.Urls
      .Select(x => $"- [{x.Key}]({x.Value})")
      .ToList();

    var description = ev.Message;
    if (links.Count != 0) {
      description += "\n\n---\n";
      description += string.Join("\n", links);
    }
    
    var embed = new Dictionary<string, object?> {
      ["title"] = $"{ev.Emoji} {ev.Title}",
      ["description"] = description,
      ["color"] = 5814783
    };

    var payload = new Dictionary<string, object?> {
      ["username"] = "Flink",
      ["avatar_url"] = "https://gitlab.com/rtraceio/web/flink/-/raw/main/Flink/wwwroot/static/img/flink-logo.png",
      ["embeds"] = new[] { embed }
    };

    return JsonSerializer.Serialize(payload, SerializerOptions);
  }
}
