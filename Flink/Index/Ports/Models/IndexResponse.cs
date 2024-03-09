using System.Diagnostics;
using System.Reflection;

namespace Flink.Index.Ports.Models;

public class IndexResponse {
  public string ApplicationName => "Flink";
  public string ApplicationRepository => "https://gitlab.com/rtraceio/web/flink";
  public string ApplicationVersion => FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion ?? "Unknown";
}
