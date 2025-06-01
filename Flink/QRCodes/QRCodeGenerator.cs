
using QRCoder;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace Flink.QRCodes;

public static class QrCodeGenerator {
  public static byte[] CreateUriQrCode(string uri, int length) {
    var payload = new PayloadGenerator.Url(uri).ToString();
    var qrGenerator = new QRCodeGenerator();
    var qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
    var qrCode = new PngByteQRCode(qrCodeData);
    var qrCodeImage = Image.Load(qrCode.GetGraphic(40));
    qrCodeImage.Mutate(x => x.Resize(length, length));
    return EncodeToRawPng(qrCodeImage);
  }

  private static byte[] EncodeToRawPng(Image img) {
    using var stream = new MemoryStream();
    img.Save(stream, new PngEncoder());
    return stream.ToArray();
  }
}
