using System.Diagnostics.Metrics;

namespace Flink.UnitTests.TestSupport;

public class UnitTestMeterFactory : IMeterFactory {
  public void Dispose() {
    GC.SuppressFinalize(this);
  }

  public Meter Create(MeterOptions options) {
    return new Meter(options);
  }
}
