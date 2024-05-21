namespace Flink.Shared.Extensions;

public static class AsyncExtensions {
  public static Task<TTaskResult[]> AwaitAll<TTaskResult>(this IEnumerable<Task<TTaskResult>> tasksToAwait) =>Task.WhenAll(tasksToAwait);
}
