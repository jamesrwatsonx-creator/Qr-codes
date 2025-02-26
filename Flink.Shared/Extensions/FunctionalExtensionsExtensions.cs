using CSharpFunctionalExtensions;

namespace Flink.Shared.Extensions;

public static class FunctionalExtensionsExtensions {
  public static Maybe<T> TapSome<T>(this Maybe<T> maybeValue, Action<T> action) {
    if (maybeValue.HasValue) action(maybeValue.Value);
    return maybeValue;
  }

  public static Maybe<T> TapNone<T>(this Maybe<T> maybeValue, Action action) {
    if (maybeValue.HasNoValue) action();
    return maybeValue;
  }

  public static Maybe<T> CompensateNone<T>(this Maybe<T> maybeValue, Func<T> compensation) {
    return maybeValue.HasNoValue
      ? compensation()
      : maybeValue;
  }
}
