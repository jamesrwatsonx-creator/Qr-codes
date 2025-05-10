using System.Diagnostics.Metrics;
using Flink.Shared;

namespace Flink.QRCodes.Ports;

// ReSharper disable once ClassNeverInstantiated.Global
public class QrCodeMetricsRegistry {
  public const string MeterName = "qrcode";
  private const string Unit = "requests";
  private readonly Counter<long> _qrCodeAllRequests;
  private readonly Counter<long> _qrCodeOkRequests;

  public QrCodeMetricsRegistry(IMeterFactory meterFactory) {
    var meter = meterFactory.Create(MeterName, "1.0.0");
    _qrCodeOkRequests = meter
      .CreateCounter<long>("ok_get", Unit, "number of successful (HTTP 200) HTTP GET requests for specific QR code");
    _qrCodeAllRequests = meter
      .CreateCounter<long>("total_get", Unit, "total number of successful (HTTP 200) HTTP GET requests for QR codes");
  }

  public void Success(string id) {
    _qrCodeAllRequests.Add(1);
    _qrCodeOkRequests.Add(1, Tag.Create(nameof(id), id));
  }
}
