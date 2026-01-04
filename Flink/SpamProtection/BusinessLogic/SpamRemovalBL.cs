using Flink.Events.Ports;
using Flink.ShortLinks.Ports;
using Flink.SpamProtection.Ports.Events;
using Flink.SpamProtection.Ports.Repositories;

namespace Flink.SpamProtection.BusinessLogic;

public static class SpamRemovalBl {
  public static async Task HandleBlockedLinkRemovalAsync(
    IShortLinkRepository shortLinkRepository,
    IBlockedHostRepository blockedHostRepository,
    IEventNotificationQueue eventNotificationQueue) {

    var shortLinkSnapshot = await shortLinkRepository.GetAll();
    if (shortLinkSnapshot.IsFailure) return;

    foreach (var shortLink in shortLinkSnapshot.Value) {
      var host = new Uri(shortLink.Uri).Host.ToLowerInvariant();
      var isHostBlocked = await blockedHostRepository.Contains(host);
      if (isHostBlocked) {
        await shortLinkRepository.DeleteAsync(shortLink.Id);
        eventNotificationQueue.Publish(new SpamProtectionLinkBlockEvent() {
          Emoji = "🛡️",
          Title = "Link Blocked",
          Message =
            $"Flink Spam Protection blocked Link with '{shortLink.Id}', targeting {shortLink.Uri} (created by {shortLink.Creator} at {shortLink.CreationDate}",
          ImageUrl = null,
          Urls = new() {
            { "Long Link", new Uri(shortLink.Uri) },
          }
        });
      }
    }
  }
}
