using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Flink.IntegrationTests;

public class IndexTests(WebApplicationFactory<Program> webApplicationFactory) : IClassFixture<WebApplicationFactory<Program>> {
  [Fact]
  public async Task IndexPageRequest_NoParametersPassed_Returns200() {
    var r = await webApplicationFactory
      .CreateClient()
      .GetAsync("/");

    r.StatusCode.Should().Be(HttpStatusCode.OK);
  }
  
  [Fact]
  public async Task EmbedRequest_NoParametersPassed_Returns200() {
    var r = await webApplicationFactory
      .CreateClient()
      .GetAsync("/embed");

    r.StatusCode.Should().Be(HttpStatusCode.OK);
  }
}
