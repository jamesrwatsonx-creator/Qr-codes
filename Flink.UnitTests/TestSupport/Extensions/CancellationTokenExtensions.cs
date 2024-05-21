namespace Flink.UnitTests.TestSupport.Extensions;

public static class CancellationTokenExtensions {
  public static async Task WaitUntilCanceled(this CancellationToken ct) {
    try {
      while (!ct.IsCancellationRequested) {
        await Task.Delay(TimeSpan.FromMilliseconds(100), ct);
        return;
      }
    } catch(TaskCanceledException){}
  }
}
