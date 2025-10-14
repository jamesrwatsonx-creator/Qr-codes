// ReSharper disable InconsistentNaming

namespace Flink.GlobalProxy.Ports.Models;

public class ProxyConfiguration {
  public string? Http_Proxy { get; init; } = null;
  public string? Https_Proxy { get; init; } = null;
  public string? Socks_Proxy { get; init; } = null;
  public string? No_Proxy { get; init; } = null;
}
