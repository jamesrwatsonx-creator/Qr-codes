using CSharpFunctionalExtensions;
using Flink.Domain;
using Flink.ShortLinks.Ports;
using Flink.SpamProtection.Repositories;
using Flink.SpamProtection.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Flink.UnitTests.SpamProtection;

public class SpamRemovalServiceTests {
  [Fact]
  public async Task SpamFlinkified_ClassifiedAsSpamInRetrospect_Removed() {
    // Arrange
    var forbiddenDomains = new ForbiddenDomainRepository();
    var shortLinkRepoMock = new Mock<IShortLinkRepository>();

    shortLinkRepoMock
      .Setup(x => x.GetAll())
      .ReturnsAsync(Result.Success<List<ShortenedLink>>([
        new ShortenedLink { Id = "spam", ActiveFrom = 0, ActiveUntil = long.MaxValue, CreationDate = 0, Creator = "spammer", Uri = "https://spam.spam.tld" }
      ]));
    
    shortLinkRepoMock
      .Setup(x => x.DeleteAsync(It.IsAny<string>()))
      .ReturnsAsync(Result.Success());
    
    var provider = new ServiceCollection()
      .AddSingleton(forbiddenDomains)
      .AddSingleton(shortLinkRepoMock.Object)
      .BuildServiceProvider();
    var scopeFactory = provider.GetRequiredService<IServiceScopeFactory>();

    
    var spamRemovalService = new SpamRemovalService(NullLogger<SpamRemovalService>.Instance, forbiddenDomains, scopeFactory);
    forbiddenDomains.AddForbiddenDomains(["spam.spam.tld"]);
    var cts = new CancellationTokenSource(TimeSpan.FromSeconds(2000));
    
    // Act
    try {
      await spamRemovalService.RunOnce();
      await Task.Delay(TimeSpan.FromSeconds(1), cts.Token);
    } catch(TaskCanceledException) { /* ignored */ }

    // Assert
    shortLinkRepoMock.Verify(x => x.DeleteAsync(It.Is<string>(m => m == "spam")), Times.Once);
  }
}
