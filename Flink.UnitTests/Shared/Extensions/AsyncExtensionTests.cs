using Flink.Shared.Extensions;
using FluentAssertions;

namespace Flink.UnitTests.Shared.Extensions;

public class AsyncExtensionTests {
  [Fact]
  public async Task MultipleFinishedTasks_Awaited_ReturnResults() {
    // Arrange
    var tasks = new List<Task<int>> {
      Task.FromResult(0),
      Task.FromResult(1),
      Task.FromResult(2)
    };

    // Act
    var results = await tasks.AwaitAll();
    
    // Assert
    results.Should().BeEquivalentTo([0, 1, 2]);
  }

  [Fact]
  public async Task MultipleDictionaryEntries_Awaited_ReturnResults() {
    // Arrange
    var dict = new Dictionary<string, Task<string>>() {
      { "gulasch", Task.FromResult("gulasch") },
      { "food", Task.FromResult("food") },
      { "1", Task.FromResult("1") }
    };
    
    // Act
    var r = await dict.AwaitValueTasks();

    // Assert
    foreach (var kv in r) {
      kv.Key.Should().Be(kv.Value);
    }
  }
}
