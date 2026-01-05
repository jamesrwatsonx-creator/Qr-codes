namespace Flink.Shared.Extensions;

public static class AsyncExtensions {
  public static Task<TTaskResult[]> AwaitAll<TTaskResult>(this IEnumerable<Task<TTaskResult>> tasksToAwait) {
    return Task.WhenAll(tasksToAwait);
  }

  public static async Task<Dictionary<TKey, TValue>> AwaitValueTasks<TKey, TValue>(this IDictionary<TKey, Task<TValue>> d) where TKey : notnull {
    await Task.WhenAll(d.Values); // once awaited, it's safe to access ".Result" on the task
    return d.ToDictionary(kv => kv.Key, kv => kv.Value.Result);
  }
}
