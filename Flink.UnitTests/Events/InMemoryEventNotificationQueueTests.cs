using Flink.Events;
using Flink.Events.Ports;
using FluentAssertions;

namespace Flink.UnitTests.Events;

public class InMemoryEventNotificationQueueTests {
  private readonly EventNotification _testEventNotification = new(FlinkEventNotificationType.FlinkBooted) {
    Emoji = "",
    ImageUrl = "",
    Message = "",
    Title = "",
    Urls = new Dictionary<string, Uri>()
  };

  [Fact]
  public void SingleEventPublished_Publishing_IsSuccessful() {
    // Arrange
    var q = new InMemoryEventNotificationNotificationQueue();

    // Act
    var result = q.Publish(_testEventNotification);

    // Assert
    result.IsSuccess.Should().BeTrue();
  }

  [Fact]
  public void SingleEventPublished_ConsumeMultiple_ReturnsSingle() {
    // Arrange
    var q = new InMemoryEventNotificationNotificationQueue();
    q.Publish(_testEventNotification);

    // Act
    var dequeued = q
      .Dequeue<EventNotification>(10)
      .ToList();

    // Assert
    dequeued.Should().HaveCount(1);
    dequeued.Should().ContainEquivalentOf(_testEventNotification);
  }

  [Fact]
  public void NoEventPublished_ConsumeMultiple_ReturnsEmpty() {
    // Arrange
    var q = new InMemoryEventNotificationNotificationQueue();

    // Act
    var dequeued = q
      .Dequeue<EventNotification>(10)
      .ToList();

    // Assert
    dequeued.Should().HaveCount(0);
  }
}
