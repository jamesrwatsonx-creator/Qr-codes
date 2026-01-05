using CSharpFunctionalExtensions;
using Flink.Domain;
using Flink.Events.Ports;
using Flink.ShortLinks.Ports;
using Flink.SpamProtection.BusinessLogic;
using Flink.SpamProtection.Repositories;
using Moq;

namespace Flink.UnitTests.SpamProtection;

public class SpamRemovalBlTests {
  [Fact]
  public async Task NoShortenedLink_WithoutBlockListEntries_DoesNotEmitEventAndDoesNotDelete() {
    // Arrange
    var shortLinkRepoMock = new Mock<IShortLinkRepository>();
    shortLinkRepoMock
      .Setup(x => x.GetAll())
      .ReturnsAsync(Result.Success<List<ShortenedLink>>([]));
    var eventQueueMock = new Mock<IEventNotificationQueue>();
    eventQueueMock
      .Setup(x => x.Publish(It.IsNotNull<EventNotification>()))
      .Returns(Result.Success());
    var blockedHostRepo = new BlockedHostInMemoryRepository();
    
    // Act
    await SpamRemovalBl.HandleBlockedLinkRemovalAsync(shortLinkRepoMock.Object, blockedHostRepo, eventQueueMock.Object);
    
    // Assert
    shortLinkRepoMock.Verify(x => x.GetAll(), Times.Once);
    eventQueueMock.Verify(x => x.Publish(It.IsNotNull<EventNotification>()), Times.Never);
  }
  
  [Fact]
  public async Task ShortenedLinkBlocklisted_WithBlockListEntry_EmitsSingleEvent() {
    // Arrange
    var shortenedLink = new ShortenedLink {
      Id = "hello",
      Uri = "https://easylist.to/",
      CreationDate = 0,
      Creator = "ruffy",
      ActiveFrom = 0,
      ActiveUntil = 0,
      PasswordHash = null
    };
    var shortLinkRepoMock = new Mock<IShortLinkRepository>();
    shortLinkRepoMock
      .Setup(x => x.GetAll())
      .ReturnsAsync(Result.Success<List<ShortenedLink>>([ shortenedLink ]));
    var eventQueueMock = new Mock<IEventNotificationQueue>();
    eventQueueMock
      .Setup(x => x.Publish(It.IsNotNull<EventNotification>()))
      .Returns(Result.Success());
    var blockedHostRepo = new BlockedHostInMemoryRepository();
    await blockedHostRepo.InsertAsync(["easylist.to"]);
    
    // Act
    await SpamRemovalBl.HandleBlockedLinkRemovalAsync(shortLinkRepoMock.Object, blockedHostRepo, eventQueueMock.Object);
    
    // Assert
    shortLinkRepoMock.Verify(x => x.GetAll(), Times.Once);
    shortLinkRepoMock.Verify(x => x.DeleteAsync(It.Is<string>(s => s == shortenedLink.Id)), Times.Once);
    eventQueueMock.Verify(x => x.Publish(It.IsNotNull<EventNotification>()), Times.Once);
  }
}
