using System.Text;
using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace Flink.Shared;

public class Credential {
  public required string Username { get; init; }
  public required string Password { get; init; }

  public bool Equals(Credential other) {
    return Username == other.Username && Password == other.Password;
  }

  public override bool Equals(object? obj) {
    if (obj is null) return false;
    if (ReferenceEquals(this, obj)) return true;
    return obj.GetType() == GetType() && Equals((Credential)obj);
  }

  public override int GetHashCode() {
    return HashCode.Combine(Username, Password);
  }
}

public static class BasicAuth {
  private static readonly Regex HeaderRegex = new(@"^Basic [A-Za-z0-9+/=]+(?![\r\n])$", RegexOptions.Compiled | RegexOptions.Singleline,
    TimeSpan.FromSeconds(1));

  private static readonly Dictionary<string, Func<string, bool>> UsernamePasswordCriteriaCatalog = new() {
    { "Not null, not empty", (u) => !string.IsNullOrEmpty(u) },
    { "Maximum Length", (u) => u.Length < 200 },
    { "Minimum Length", (u) => u.Length >= 3 }
  };

  public static Result<Credential> GetCredential(string? authorizationHeaderValue) {
    if (authorizationHeaderValue == null)
      return Result.Failure<Credential>("No Basic Auth Header provided.");
    if (!HeaderRegex.IsMatch(authorizationHeaderValue))
      return Result.Failure<Credential>("Basic Auth Header malformed. Cannot parse authorization header.");

    var credentialBytes = Convert.FromBase64String(authorizationHeaderValue.Replace("Basic ", ""));
    var credentials = Encoding.UTF8.GetString(credentialBytes).Trim().Split(':', 2);
    if (UsernamePasswordCriteriaCatalog.Any(kvp => kvp.Value(credentials[0]) == false)) {
      var err = UsernamePasswordCriteriaCatalog.First(kvp => kvp.Value(credentials[0]) == false).Key;
      return Result.Failure<Credential>($"Username is invalid. Constraint '{err}' violated");
    }

    // ReSharper disable once InvertIf
    if (UsernamePasswordCriteriaCatalog.Any(kvp => kvp.Value(credentials[1]) == false)) {
      var err = UsernamePasswordCriteriaCatalog.First(kvp => kvp.Value(credentials[1]) == false).Key;
      return Result.Failure<Credential>($"Password is invalid. Constraint '{err}' violated");
    }

    return new Credential {
      Username = credentials[0],
      Password = credentials[1]
    };
  }
}
