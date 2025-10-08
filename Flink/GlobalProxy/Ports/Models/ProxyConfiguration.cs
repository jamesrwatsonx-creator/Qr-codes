using System.ComponentModel.DataAnnotations;
// ReSharper disable InconsistentNaming

namespace Flink.GlobalProxy.Ports.Models;

public class ProxyConfiguration {
  [Url]
  public required string? Http_Proxy { get; init; } = null;
  [Url]
  public required string? Https_Proxy { get; init; } = null;
  [Url]
  public required string? Socks_Proxy { get; init; } = null;
}
