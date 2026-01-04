using System.Net;
using System.Text.Json.Serialization;

namespace Flink.Shared.ApiResponse;

public class ApiResponse<T> {
  private ApiResponse() { }
  public bool IsSuccess { get; private init; }
  public string StatusText { get; private init; } = "";
  [JsonIgnore]
  public int HttpStatusCode { get; private init; }
  [JsonIgnore]
  public string? Location { get; private init; }
  public string? Message { get; private init; }
  public DateTimeOffset DateTime { get; private init; }
  public bool HasData => Data != null;
  // ReSharper disable once MemberCanBePrivate.Global
  public T? Data { get; private init; }

  public static ApiResponse<T> Create(HttpStatusCode httpStatusCode, T? data, string? message=null, string? location=null) {
    return new ApiResponse<T> {
      IsSuccess = (int)httpStatusCode is >= 200 and <= 299,
      StatusText = httpStatusCode.ToString(),
      HttpStatusCode = (int)httpStatusCode,
      Data = data,
      DateTime = DateTimeOffset.UtcNow,
      Message = message,
      Location = location
    };
  }
}
