using System.Net;
using Flink.Shared.ApiResponse;
using Microsoft.AspNetCore.Mvc;

namespace Flink;

public static class ApiResponseExtensions {
  public static IActionResult ToResult<T>(this ApiResponse<T> r) {
    if (r is { Location: not null, HttpStatusCode: (int)HttpStatusCode.Created }) {
      return new CreatedResult(r.Location, r);
    }
    
    return new ObjectResult(r) {
      StatusCode = r.HttpStatusCode
    };
  }
}
