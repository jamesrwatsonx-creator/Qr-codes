using System.Net;
using Flink.GlobalProxy.Ports.Models;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;

namespace Flink.GlobalProxy;

public class GlobalProxyHandlerBuilderFilter(IOptions<ProxyConfiguration> proxyOptions) : IHttpMessageHandlerBuilderFilter {
  public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next) {
    return builder => {
      next(builder);
      var proxy = proxyOptions.Value.Http_Proxy ?? proxyOptions.Value.Https_Proxy ?? proxyOptions.Value.Socks_Proxy;
      var proxyEnabled = !string.IsNullOrEmpty(proxy);
      if (builder.PrimaryHandler is not HttpClientHandler handler) return;
      handler.Proxy = proxyEnabled ? new WebProxy(proxy) : null;
      handler.UseProxy = proxyEnabled;
    };
  }
}
