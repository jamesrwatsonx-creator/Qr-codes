using Flink.SpamProtection.Repositories;
using FluentAssertions;

namespace Flink.UnitTests.SpamProtection;

public class BlockedHostInMemoryRepositoryTests {
  [Fact]
  public async Task NewlyCreatedHashSet_QueryCount_IsEmpty() {
    // Arrange
    var r = new BlockedHostInMemoryRepository();
    
    // Act
    var count = await r.CountAsync();
    
    // Assert
    count.Should().Be(0);
  }
  
  [Fact]
  public async Task OneElementInserted_QueryCount_IsOne() {
    // Arrange
    var r = new BlockedHostInMemoryRepository();
    
    // Act
    await r.InsertAsync(["awesome.domain.tld"]);
    var count = await r.CountAsync();
    
    // Assert
    count.Should().Be(1);
  }
  
  [Fact]
  public async Task OneElementInserted_ContainsCheck_IsTrue() {
    // Arrange
    var id = Guid.NewGuid().ToString();
    var r = new BlockedHostInMemoryRepository();
    
    // Act
    await r.InsertAsync([id]);
    var isContained = await r.Contains(id);
    
    // Assert
    isContained.Should().BeTrue();
  }
  
  [Fact]
  public async Task OneElementInsertedTwiceAtInsert_QueryCount_IsOne() {
    // Arrange
    var id = Guid.NewGuid().ToString();
    var r = new BlockedHostInMemoryRepository();
    
    // Act
    await r.InsertAsync([id, id]);
    var count =  await r.CountAsync();
    
    // Assert
    count.Should().Be(1);
  }
  
  [Fact]
  public async Task OneElementInsertedTwiceSubsequently_QueryCount_IsOne() {
    // Arrange
    var id = Guid.NewGuid().ToString();
    var r = new BlockedHostInMemoryRepository();
    
    // Act
    await r.InsertAsync([id]);
    await r.InsertAsync([id]);
    var count =  await r.CountAsync();
    
    // Assert
    count.Should().Be(1);
  }
  
  
  [Fact]
  public async Task HashSetFilledToExpectedCountAlready_InsertOneMore_AddsOne() {
    // Arrange
    var r = new BlockedHostInMemoryRepository();
    var blocks = Enumerable.Range(0, BlockedHostInMemoryRepository.HashsetCapacity).Select(i => $"{i}");
    await r.InsertAsync(blocks);
    
    // Act
    await r.InsertAsync([Guid.NewGuid().ToString()]);
    var count = await r.CountAsync();
    
    // Assert
    count.Should().Be(BlockedHostInMemoryRepository.HashsetCapacity + 1);
  }
}
