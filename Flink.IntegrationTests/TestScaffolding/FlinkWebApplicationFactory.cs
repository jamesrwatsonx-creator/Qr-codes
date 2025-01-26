using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Flink.IntegrationTests.TestScaffolding;

// ReSharper disable once ClassNeverInstantiated.Global
public class FlinkWebApplicationFactory : WebApplicationFactory<Program> {
  protected override void ConfigureWebHost(IWebHostBuilder builder) {      
    builder.ConfigureServices(services => {
      builder.UseEnvironment("Flink.IntegrationTests");
    });
  }
}
