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
  public string ApplicationVersion => FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion ?? "Unknown";
}
