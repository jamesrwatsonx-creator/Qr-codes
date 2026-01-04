using Flink.Shared;
using Flink.Shared.DataStructures;
using FluentAssertions;

namespace Flink.UnitTests.Shared.DataStructures;

public class ConcurrentHashSetTests {
  [Fact]
  public void NewlyCreatedHashSet_QueryCount_IsEmpty() {
    // Arrange, Act
    var hs = new ConcurrentHashSet<string>(1, StringComparer.OrdinalIgnoreCase);
    
    // Assert
    hs.Count().Should().Be(0);
  }
  
  [Fact]
  public void OneElementInserted_QueryCount_IsOne() {
    // Arrange
    var hs = new ConcurrentHashSet<string>(1, StringComparer.OrdinalIgnoreCase);
    
    // Act
    hs.Insert(["Hello"]);
 
    // Assert
    hs.Count().Should().Be(1);
  }

  [Fact]
  public void OneElementInsertedTwiceAtInsert_QueryCount_IsOne() {
    // Arrange
    var id = Guid.NewGuid().ToString();
    var hs = new ConcurrentHashSet<string>(1, StringComparer.OrdinalIgnoreCase);
    
    // Act
    hs.Insert([id, id]);
 
    // Assert
    hs.Count().Should().Be(1);
  }
  
  [Fact]
  public void OneElementInsertedTwiceSubsequently_QueryCount_IsOne() {
    // Arrange
    var id = Guid.NewGuid().ToString();
    var hs = new ConcurrentHashSet<string>(1, StringComparer.OrdinalIgnoreCase);
    
    // Act
    hs.Insert([id]);
    hs.Insert([id]);
 
    // Assert
    hs.Count().Should().Be(1);
  }
  
  [Fact]
  public void OneElementInserted_ContainsCheck_IsTrue() {
    // Arrange
    var id = Guid.NewGuid().ToString();
    var hs = new ConcurrentHashSet<string>(1, StringComparer.OrdinalIgnoreCase);
    
    // Act
    hs.Insert([id]);
 
    // Assert
    hs.Contains(id).Should().BeTrue();
  }

  [Fact]
  public void HashSetFilledToExpectedCountAlready_InsertOneMore_AddsOne() {
    // Arrange
    var hs = new ConcurrentHashSet<string>(1, StringComparer.OrdinalIgnoreCase);
    
    // Act
    hs.Insert([Guid.NewGuid().ToString()]);
    hs.Insert([Guid.NewGuid().ToString()]);
 
    // Assert
    hs.Count().Should().Be(2);
  }
}
