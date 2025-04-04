using Flink.Shared.Extensions;
using FluentAssertions;

namespace Flink.UnitTests.Shared;

public class AsyncExtensionTests {
  [Fact]
  public async Task MultipleFinishedTasks_Awaited_ReturnResults() {
    var tasks = new List<Task<int>> {
      Task.FromResult(0),
      Task.FromResult(1),
      Task.FromResult(2),
    };

    var results = await tasks.AwaitAll();
    results.Should().BeEquivalentTo([0, 1, 2]);
  }
}
