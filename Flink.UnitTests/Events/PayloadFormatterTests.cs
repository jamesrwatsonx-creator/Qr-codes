using Flink.Events.PayloadFormatters;
using Flink.Events.Ports;
using Flink.Events.Ports.Models;

namespace Flink.UnitTests.Events;

public class PayloadFormatterTests {
  private readonly EventNotification _eventNotification = new EventNotification(FlinkEventNotificationType.FlinkLinkShortened) {
    Emoji = "🆕",
    Title = "Payload Formatter Test",
    Message = "Hello World 01234567890äöüß",
    ImageUrl = "https://blog.rtrace.io/favicon.ico",
    Urls = new Dictionary<string, Uri> {
      { "Long Link", new Uri("https://blog.rtrace.io") },
      { "Short URL", new Uri("https://blog.rtrace.io") },
      { "Embed", new Uri("https://blog.rtrace.io") }
    },
  };

  [Fact]
  public Task Verify_DiscordPayload_Matches() {
    var config = new EventNotificationSinkConfiguration {
      Method = "POST",
      Name = "unit-test",
      PayloadFormat = "discord",
      Url = "https://discord.com/somewebhook",
      Events = [FlinkEventNotificationType.FlinkBooted]
    };
    var payloadFormatter = new DiscordFormatter();

    var payload = payloadFormatter.Format(_eventNotification, config);
    return Verify(payload);
  }

  [Fact]
  public Task Verify_SlackPayload_Matches() {
    var config = new EventNotificationSinkConfiguration {
      Method = "POST",
      Name = "unit-test",
      PayloadFormat = "slack",
      Url = "https://slack.com/somewebhook",
      Events = [FlinkEventNotificationType.FlinkBooted]
    };
    var payloadFormatter = new MattermostSlackRocketChatPayloadFormatter();

    var payload = payloadFormatter.Format(_eventNotification, config);
    return Verify(payload);
  }

  [Fact]
  public Task Verify_NtfyPayload_Matches() {
    var config = new EventNotificationSinkConfiguration {
      Method = "POST",
      Name = "unit-test",
      PayloadFormat = "ntfy",
      Url = "https://ntfy.rtrace.io/randomtopic",
      Events = [FlinkEventNotificationType.FlinkBooted]
    };
    var payloadFormatter = new MattermostSlackRocketChatPayloadFormatter();

    var payload = payloadFormatter.Format(_eventNotification, config);
    return Verify(payload);
  }

  [Fact]
  public Task Verify_MattermostPayload_Matches() {
    var config = new EventNotificationSinkConfiguration {
      Method = "POST",
      Name = "unit-test",
      PayloadFormat = "mattermost",
      Url = "https://mattermost.com/somewebhook",
      Events = [FlinkEventNotificationType.FlinkBooted]
    };
    var payloadFormatter = new MattermostSlackRocketChatPayloadFormatter();

    var payload = payloadFormatter.Format(_eventNotification, config);
    return Verify(payload);
  }

  [Fact]
  public Task Verify_RocketchatPayload_Matches() {
    var config = new EventNotificationSinkConfiguration {
      Method = "POST",
      Name = "unit-test",
      PayloadFormat = "rocketchat",
      Url = "https://rocket.chat/somewebhook",
      Events = [FlinkEventNotificationType.FlinkBooted]
    };
    var payloadFormatter = new MattermostSlackRocketChatPayloadFormatter();

    var payload = payloadFormatter.Format(_eventNotification, config);
    return Verify(payload);
  }
}
