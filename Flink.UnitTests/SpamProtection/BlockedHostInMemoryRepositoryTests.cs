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

  [Theory]
  [InlineData("sub.")]
  [InlineData("sub.sub.")]
  [InlineData("sub.sub.sub.")]
  public async Task ParentDomainIsBlocked_SubDomainChecked_IsBlocked(string subdomains) {
    // Arrange
    const string ParentDomain = "awesome.domain.tld";
    var r = new BlockedHostInMemoryRepository();
    await r.InsertAsync([ParentDomain]);
    
    // Act
    var result = await r.Contains($"{subdomains}{ParentDomain}");
    
    // Assert
    result.Should().BeTrue();
  }
  
  [Fact]
  public async Task ChildDomainIsBlocked_ParentDomainChecked_IsUnblocked() {
    // Arrange
    const string ParentDomain = "awesome.domain.tld";
    var r = new BlockedHostInMemoryRepository();
    await r.InsertAsync([ParentDomain]);
    
    // Act
    var result = await r.Contains("domain.tld");
    
    // Assert
    result.Should().BeFalse();
  }
}
