using Flink.MetaData.Database;
using Flink.ShortLinks.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace Flink.IntegrationTests.TestScaffolding;

// ReSharper disable once ClassNeverInstantiated.Global
public class FlinkWebApplicationFactory : WebApplicationFactory<Program> {
  protected override void ConfigureWebHost(IWebHostBuilder builder) {      

    
    builder.ConfigureServices(services => {
      using var scope = services.BuildServiceProvider().CreateScope();
      scope.ServiceProvider.GetRequiredService<ShortenedLinkDbContext>().Database.EnsureCreated();
      scope.ServiceProvider.GetRequiredService<MetaDataDbContext>().Database.EnsureCreated();
      builder.UseEnvironment("Flink.IntegrationTests");
    });
  }


}
