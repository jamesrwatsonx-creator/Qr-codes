namespace Flink.MetaData.Extensions;

public static class DictionaryExtensions {
  private static Tv? GetOrDefault<Tk, Tv>(this IDictionary<Tk, Tv> dict, Tk key, Tv? fallback) {
    return dict.TryGetValue(key, out var value) 
      ? value
      : fallback;
  }

  public static Tv? GetOrDefault<Tk, Tv>(this IDictionary<Tk, Tv> dict, Tk[] keys, Tv? fallback) {
    foreach (var key in keys) {
      var val = dict.GetOrDefault(key, fallback);
      if (val?.Equals(fallback) == false)
        return val;
    }

    return fallback;
  }
}
