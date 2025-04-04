using System.ComponentModel.DataAnnotations;
using Flink.Shared.Extensions;
using FluentAssertions;

namespace Flink.UnitTests.Shared;

public class TestClass() {
  [Range(0, 10)]
  public int Value { get; init; }
}

public class ConfigValidationTests {
  [Fact]
  public void ConfigValidation_NullModel_Throws() {
    // Arrange
    TestClass? configModel = null;

    // Act & Assert
    try {
      ConfigValidationExtensions.ValidateConfiguration(configModel);
    } catch (Exception ex) {
      ex.Should().BeOfType<ArgumentNullException>();
      return;
    }

    true.Should().BeFalse();
  }
  
  [Theory]
  [InlineData(-1)]
  [InlineData(11)]
  public void ConfigValidation_WrongModel_Throws(int value) {
    // Arrange
    var configModel = new TestClass { Value = value };

    // Act & Assert
    configModel
      .Invoking(c => c.ValidateConfiguration())
      .Should()
      .ThrowExactly<ApplicationException>();
  }

  [Fact]
  public void ConfigValidation_ValidModel_ReturnsInputModel() {
    // Arrange
    var configModel = new TestClass { Value = 2 };

    // Act 
    var model = configModel.ValidateConfiguration();
    
    // Assert
    model.Should().BeEquivalentTo(configModel);
  }
}
