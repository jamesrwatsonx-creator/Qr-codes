using CSharpFunctionalExtensions;

namespace Flink.QRCodes.Repositories;

public interface IQrCodeCache {
  public Result<byte[]> Cache(string id, int height, byte[] data);
  public Result<Maybe<byte[]>> Retrieve(string id, int height);
}
