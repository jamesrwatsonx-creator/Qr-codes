using Flink.Shared;
using FluentAssertions;

namespace Flink.UnitTests.Shared;

public class PasswordHasherTests {
  [Theory]
  [InlineData("")]
  [InlineData(" ")]
  [InlineData("1")]
  [InlineData("öüäz")]
  [InlineData("@/€&`")]
  [InlineData(null)]
  public void PasswordHasher_CalculateSha256AndVerifyHash_ReturnsTrue(string? password) {
    var hash = PasswordHasher.CalculateSha256PasswordHash(password, "static");
    PasswordHasher.VerifyPasswordHash(hash, password, "static").Should().BeTrue();
  }
  
  [Theory]
  [InlineData("")]
  [InlineData(" ")]
  [InlineData("1")]
  [InlineData("öüäz")]
  [InlineData("@/€&`")]
  [InlineData(null)]
  public void PasswordHasher_CalculateShake256AndVerifyHash_ReturnsTrue(string? password) {
    var hash = PasswordHasher.CalculateShake256PasswordHash(password, "static");
    PasswordHasher.VerifyPasswordHash(hash, password, "static").Should().BeTrue();
  }
  
  [Theory]
  [InlineData("")]
  [InlineData(" ")]
  [InlineData("1")]
  [InlineData("öüäz")]
  [InlineData("@/€&`")]
  [InlineData(null)]
  public void PasswordHasher_CalculateSAndVerifyHash_ReturnsTrue(string? password) {
    var hash = PasswordHasher.CalculatePasswordHash(password, "static");
    PasswordHasher.VerifyPasswordHash(hash, password, "static").Should().BeTrue();
  }
  
  [Fact]
  public void PasswordHasherUserPassesNull_VerifyHash_ReturnsFalse() {
    var hash = PasswordHasher.CalculatePasswordHash("password", "static");
    PasswordHasher.VerifyPasswordHash(hash, null, "static").Should().BeFalse();
  }
  
  [Fact]
  public void PasswordHasher_CalculateHash_DefaultsToShake256() {
    var hash = PasswordHasher.CalculatePasswordHash("password", "static");
    hash.Should().StartWith("shake256:32");
  }
  
  [Theory]
  [InlineData("")]
  [InlineData(" ")]
  [InlineData("nonexistentHashFunction:32|34987239840723094723")]
  [InlineData("shake256:32,234098304")]
  [InlineData("shake256-32,234098304")]
  [InlineData("sha256:64|sdf29304823")]
  public void PasswordHasherWithMalformedDbHash_VerifyHash_ReturnsFalse(string? malformedDbHash) {
    PasswordHasher.VerifyPasswordHash(malformedDbHash, "password", "static").Should().BeFalse();
  }
}
