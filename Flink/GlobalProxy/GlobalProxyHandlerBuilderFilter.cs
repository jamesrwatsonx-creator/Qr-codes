using System.Net;
using Flink.GlobalProxy.Ports.Models;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;

namespace Flink.GlobalProxy;

public class GlobalProxyHandlerBuilderFilter(IOptions<ProxyConfiguration> proxyOptions) : IHttpMessageHandlerBuilderFilter {
  public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next) {
    return builder => {
      next(builder);
      var proxy = proxyOptions.Value.Socks_Proxy.EmptyToNull()
                  ?? proxyOptions.Value.Https_Proxy.EmptyToNull()
                  ?? proxyOptions.Value.Http_Proxy.EmptyToNull();
      var proxyEnabled = !string.IsNullOrEmpty(proxy);
      if (builder.PrimaryHandler is not SocketsHttpHandler handler) return;
      handler.Proxy = proxyEnabled ? new WebProxy(proxy, true, proxyOptions.Value.No_Proxy.ParseCollection()) : null;
      handler.UseProxy = proxyEnabled;
    };
  }
}
