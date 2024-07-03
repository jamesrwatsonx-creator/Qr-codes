using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;

namespace Flink.Proxy;

public interface IForwardProxy {
  Task<Result<FileContentResult>> Proxy(string url);
}
