using Flink.SpamProtection.Models;
using Flink.SpamProtection.Repositories;
using FluentAssertions;

namespace Flink.UnitTests.SpamProtection;

public class ForbiddenDomainsRepositoryTest {
  [Theory]
  [InlineData("Domain.gov.at")]
  [InlineData("domain.gov.at")]
  [InlineData("sub.domain.gov.at")]
  [InlineData("SUB.domain.gov.at")]
  [InlineData("suB2.sub1.domain.gov.at")]
  public void ForbiddenDomainsConfigured_ContainsDomainCalledWithMatchingDomains_ReturnsTrue(string id) {
    // Arrange
    var repo = new ForbiddenDomainRepository();
    repo.AddForbiddenDomains(["domain.gov.at"]);
    // Act
    var result = repo.ContainsDomain(id);
    // Assert
    result.Should().BeTrue();
  }

  [Fact]
  public void NoDefinitionSetup_NewDefinitionInsert_ReflectsInList() {
    // Arrange
    var repo = new ForbiddenDomainRepository();
    var source = new EasyListSource {
      LastModified = DateTime.UtcNow,
      Source = "",
      Title = "",
      Version = 0
    };
    // Act
    repo.AddDefinition(source);

    // Assert
    repo.UsedEasyListSources.Should().ContainEquivalentOf(source);
  }

  [Fact]
  public void NoDefinitionSetup_TwiceInserted_ReflectsOnceInList() {
    // Arrange
    var repo = new ForbiddenDomainRepository();
    var source = new EasyListSource {
      LastModified = DateTime.UtcNow,
      Source = "",
      Title = "",
      Version = 0
    };
    // Act
    repo.AddDefinition(source);
    repo.AddDefinition(source);

    // Assert
    repo.UsedEasyListSources.Should().HaveCount(1);
  }

  [Fact]
  public void DefinitionSetup_OlderVersionInserted_NotUpdatedInList() {
    // Arrange
    var repo = new ForbiddenDomainRepository();
    var sourceOlder = new EasyListSource {
      LastModified = DateTime.UtcNow,
      Source = "",
      Title = "",
      Version = 0
    };
    var sourceNewer = new EasyListSource {
      LastModified = DateTime.UtcNow,
      Source = "",
      Title = "",
      Version = 1
    };
    repo.AddDefinition(sourceNewer);

    // Act
    repo.AddDefinition(sourceOlder);

    // Assert
    repo.UsedEasyListSources.Should().HaveCount(1);
    repo.UsedEasyListSources.Single().Should().BeEquivalentTo(sourceNewer);
  }
}
