using CSharpFunctionalExtensions;
using Flink.Shared.Extensions;
using FluentAssertions;

namespace Flink.UnitTests.Shared;

public class FunctionalExtensionExtensionTests {
  [Fact]
  public void MaybeNone_WithCompensationValue_ReturnsCompensation() {
    // Arrange
    var x = Maybe<string>.None;

    // Act
    var compensated = x.CompensateNone(() => "compensation");

    // Assert
    compensated.HasValue.Should().BeTrue();
    compensated.Value.Should().Be("compensation");
  }

  [Fact]
  public void MaybeSome_WithCompensationValue_ReturnsOriginal() {
    // Arrange
    var x = Maybe.From("helloworld");

    // Act
    var compensated = x.CompensateNone(() => "compensation");

    // Assert
    compensated.HasValue.Should().BeTrue();
    compensated.Value.Should().Be("helloworld");
  }

  [Fact]
  public void TapSome_WithSome_Taps() {
    // Arrange
    var callCounter = 0;
    var x = Maybe.From("helloworld");

    // Act
    x.TapSome(t => {
      t.Should().Be("helloworld");
      callCounter++;
    });

    // Assert
    callCounter.Should().Be(1);
  }

  [Fact]
  public void TapSome_WithNone_DoesNotTap() {
    // Arrange
    var x = Maybe<string>.None;

    // Act & Arrange
    x.TapSome(_ => throw new Exception());
  }


  [Fact]
  public void TapNone_WithNone_Taps() {
    // Arrange
    var callCounter = 0;
    var x = Maybe<string>.None;

    // Act
    x.TapNone(() => { callCounter++; });

    // Assert
    callCounter.Should().Be(1);
  }

  [Fact]
  public void TapNone_WithSome_DoesNotTap() {
    // Arrange
    var x = Maybe<string>.None;

    // Act & Arrange
    x.TapSome(_ => throw new Exception());
  }
}
