namespace Flink.WebUI;

public interface IThemeManager {
  public string[] AvailableThemes();
  public string GetFile(string fileName, string? theme);
}
