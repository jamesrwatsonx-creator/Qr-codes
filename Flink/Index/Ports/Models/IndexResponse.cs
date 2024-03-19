using System.Diagnostics;
using System.Reflection;

namespace Flink.Index.Ports.Models;

public class IndexResponse {
  /// <summary>
  /// Name of the Application
  /// </summary>
  public string ApplicationName => "Flink";
  
  /// <summary>
  /// Repository of Flink
  /// </summary>
  public string ApplicationRepository => "https://gitlab.com/rtraceio/web/flink";
  
  /// <summary>
  /// The version of Flink
  /// </summary>
  public string ApplicationVersion => FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion ?? "Unknown";
  
  /// <summary>
  /// Short description explaining Flink
  /// </summary>
  public string Description => "A F(L)OSS zero-config url shortener with QR Code generator and (optional) Open Telemetry Metrics support written in ASP.NET";
}
