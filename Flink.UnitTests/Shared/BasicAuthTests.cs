using Flink.Shared;
using FluentAssertions;

namespace Flink.UnitTests.Shared;

public class BasicAuthTests {
  [Theory]
  [InlineData("Basic Z3VsYXNjaDpndWxhc2No", "gulasch", "gulasch")]
  [InlineData("Basic Zmxpbms6Zmxpbms=", "flink", "flink")]
  [InlineData("Basic QTBaMGE5ejk6ISIkJSYvKCk9Pw==", "A0Z0a9z9", "!\"$%&/()=?")]
  public void HappyPathTests(string basicAuthHeader, string username, string password) {
    var cred = BasicAuth.GetCredential(basicAuthHeader);
    cred.IsSuccess.Should().BeTrue();
    cred.Value.Username.Should().Be(username);
    cred.Value.Password.Should().Be(password);
  }
  
  [Theory]
  [InlineData(null)]
  [InlineData("")]
  [InlineData(" ")]
  [InlineData("\t")]
  [InlineData("\n")]
  [InlineData("BASIC Zmxpbms6Zmxpbms=")]
  [InlineData("basic Zmxpbms6Zmxpbms=")]
  [InlineData(" Basic Zmxpbms6Zmxpbms=")]
  [InlineData("Basic Zmxpbms6Zmxpbms= ")]
  [InlineData(" Basic Zmxpbms6Zmxpbms= ")]
  [InlineData("\nBasic Zmxpbms6Zmxpbms=")]
  [InlineData("Basic Zmxpbms6Zmxpbms=\n")]
  [InlineData("\nBasic Zmxpbms6Zmxpbms=\n")]
  [InlineData("Basic Zjpm")]
  [InlineData("Basic ZjpmYWFhYWFhYWZhZmFhYWFhYWFhYQ==")]
  [InlineData("Basic ZmFhYWFhYWFmYWZhYWFhYWFhYWE6Zg==")]
  public void InvalidHeaders_PassedToParser_ProduceResultFailure(string? basicAuthHeader) {
    var cred = BasicAuth.GetCredential(basicAuthHeader);
    cred.IsSuccess.Should().BeFalse();
  }
}
