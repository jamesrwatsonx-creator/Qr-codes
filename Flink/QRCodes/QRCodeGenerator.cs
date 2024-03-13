using QRCoder;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;

namespace Flink.QRCodes;

public static class QrCodeGenerator {
  public static byte[] CreateUriQrCode(string uri) {
    var payload = new PayloadGenerator.Url(uri).ToString();
    var qrGenerator = new QRCoder.QRCodeGenerator();
    var qrCodeData = qrGenerator.CreateQrCode(payload, QRCoder.QRCodeGenerator.ECCLevel.Q);
    var qrCode = new QRCode(qrCodeData);
    var qrCodeImage = qrCode.GetGraphic(40);
    return EncodeToRawPng(qrCodeImage);
  }

  private static byte[] EncodeToRawPng(Image img) {
    using var stream = new MemoryStream();
    img.Save(stream, new PngEncoder());
    return stream.ToArray();
  }
}
