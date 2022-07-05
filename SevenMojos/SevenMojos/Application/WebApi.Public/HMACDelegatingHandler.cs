namespace WebApi.Public;

using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Microsoft.Extensions.Options;
using Models;
public class HMACDelegatingHandler : DelegatingHandler
{
    private readonly IOptions<ApplicationSettingsModel> appSettings;
    private readonly HttpClientHandler clientHandler;

    public HMACDelegatingHandler(IOptions<ApplicationSettingsModel> appSettings)
    {
        this.appSettings = appSettings;
        this.clientHandler = new HttpClientHandler();
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var appId = this.appSettings.Value.AppId;
        var apiKey = this.appSettings.Value.ApiKey;
        
        HttpResponseMessage response = null;
        var requestContentBase64String = string.Empty;
        var requestUri = HttpUtility.UrlEncode(request.RequestUri.AbsoluteUri.ToLower());
        var requestHttpMethod = request.Method.Method;

        var epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
        var timeSpan = DateTime.UtcNow - epochStart;
        var requestTimeStamp = Convert.ToUInt64(timeSpan.TotalSeconds).ToString();

        var nonce = Guid.NewGuid().ToString("N");

        if (request.Content != null)
        {
            var content = await request.Content.ReadAsByteArrayAsync();
            var md5 = MD5.Create();
            var requestContentHash = md5.ComputeHash(content);
            requestContentBase64String = Convert.ToBase64String(requestContentHash);
        }

        var signatureRawData = string.Format("{0}{1}{2}{3}{4}{5}", appId, requestHttpMethod, requestUri,
            requestTimeStamp, nonce, requestContentBase64String);

        var secretKeyByteArray = Convert.FromBase64String(apiKey);
        var signature = Encoding.UTF8.GetBytes(signatureRawData);

        using (var hmac = new HMACSHA256(secretKeyByteArray))
        {
            var signatureBytes = hmac.ComputeHash(signature);
            var requestSignatureBase64String = Convert.ToBase64String(signatureBytes);

            request.Headers.Authorization = new AuthenticationHeaderValue("hmacauth",
                string.Format("{0}:{1}:{2}:{3}", appId, requestSignatureBase64String, nonce, requestTimeStamp));
        }
            
        this.clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;
        response = await base.SendAsync(request, cancellationToken);
        
        return response;
    }
}