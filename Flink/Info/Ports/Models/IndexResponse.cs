using System.Diagnostics;
using System.Reflection;

namespace Flink.Info.Ports.Models;

public class IndexResponse {
  /// <summary>
  ///   Name of the Application
  /// </summary>
  public string ApplicationName {
    get => "Flink";
  }

  /// <summary>
  ///   Repository of Flink
  /// </summary>
  public string ApplicationRepository {
    get => "https://gitlab.com/rtraceio/web/flink";
  }

  /// <summary>
  ///   The version of Flink
  /// </summary>
  public string ApplicationVersion {
    get => FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion ?? "Unknown";
  }

  /// <summary>
  ///   Short description explaining Flink
  /// </summary>
  public string Description {
    get => "A F(L)OSS zero-config url shortener with QR Code generator and (optional) Open Telemetry Metrics support written in ASP.NET";
  }
}
