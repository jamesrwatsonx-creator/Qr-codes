using Flink.SpamProtection.Models;
using Flink.SpamProtection.Repositories;
using FluentAssertions;

namespace Flink.UnitTests.SpamProtection;

public class BlockListSourceInMemoryRepositoryTests {
  [Fact]
  public async Task UninitializedBlockListRepository_GetSources_DoesNotContainAnySources() {
    // Arrange
    var r = new BlockListSourceInMemoryRepository();
    
    // Act
    var blockListSources = await r.GetBlockListSourcesAsync();
    
    // Assert
    blockListSources.Should().BeEmpty();
  }
  
  [Fact]
  public async Task UninitializedBlockListRepository_InsertOne_ContainsPreviouslyInserted() {
    // Arrange
    var r = new BlockListSourceInMemoryRepository();
    var blSource = new BlockListSource() {
      Title = "Hello",
      MetaData = new Dictionary<string, string>(),
      Version = 1,
      Source = "src",
      Sha3 = "1"
    };
    
    // Act
    await r.RegisterBlockListUpdateAsync(blSource);
    
    // Assert
    var blockListSources = await r.GetBlockListSourcesAsync();
    var contained = await r.IsBlockListAlreadyKnownAsync(blSource);
    contained.Should().BeTrue();
    blockListSources.Should().BeEquivalentTo([blSource]);
  }
}
