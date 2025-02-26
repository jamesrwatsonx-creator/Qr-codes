using Flink.QRCodes;
using FluentAssertions;

namespace Flink.UnitTests.QrCodes;

public class QrCodeGeneratorDeterminismTest {
  [Theory]
  [InlineData("https://blog.rtrace.io", 1024)]
  [InlineData("https://blog.rtrace.io?o=ä4", 512)]
  [InlineData("https://blog.rtrace.io?c=#e", 256)]
  [InlineData("https://blog.rtrace.io?ö=ü", 128)]
  [InlineData("https://blog.rtrace.io?X=X", 64)]
  [InlineData("https://blog.rtrace.io?R0=$&f=2", 32)]
  [InlineData("https://notexisting.domain#14123 aLLe", 44)]
  [InlineData("http://notexisting.domain/Nodd?u=2", 44)]
  public void TwoQrCodesAreGenerated_WithSameUrlAndLengthParameters_QrCodeShouldBeEqual(string uri, int length) {
    var q1 = QrCodeGenerator.CreateUriQrCode(uri, length);
    var q2 = QrCodeGenerator.CreateUriQrCode(uri, length);
    q1.SequenceEqual(q2).Should().BeTrue();
  }
}
