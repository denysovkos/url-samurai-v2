using System.Globalization;
using System.Text.Encodings.Web;

namespace UrlSamurai.Components.Services;

using QRCoder;

public class QrCodeService
{
    private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";
    private const string Issuer = "Sshare.dev";
    
    public string GenerateBase64(string text)
    {
        using var generator = new QRCodeGenerator();
        var data = generator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q);
        var base64Qr = new Base64QRCode(data);
        return base64Qr.GetGraphic(20);
    }
    
    public string GenerateTfaQr(string email, string secretKey, string issuer = Issuer)
    {
        var uri = GenerateOtpUri(email, secretKey, issuer);
        using var generator = new QRCodeGenerator();
        var data = generator.CreateQrCode(uri, QRCodeGenerator.ECCLevel.Q);
        var base64Qr = new Base64QRCode(data);
        return base64Qr.GetGraphic(20); // PNG Base64
    }

    private string GenerateOtpUri(string email, string secretKey, string issuer)
    {
        return string.Format(
            CultureInfo.InvariantCulture,
            AuthenticatorUriFormat,
            UrlEncoder.Default.Encode(issuer),
            UrlEncoder.Default.Encode(email),
            secretKey);
    }
}
