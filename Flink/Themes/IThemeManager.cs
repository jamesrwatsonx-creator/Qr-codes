namespace Flink.Themes;

public interface IThemeManager {
  public string[] AvailableThemes();
  public string GetFile(string fileName, string? theme);
}
