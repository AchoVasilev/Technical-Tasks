namespace Infrastructure;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Models;

public class HmacAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private const ulong requestMaxAgeInSeconds = 500;
    private const string authenticationScheme = "hmacauth";
    private readonly Dictionary<string, string> allowedAps = new();
    private readonly IOptions<ApplicationSettingsModel> applicationSettings;
    private readonly IMemoryCache memoryCache;

    public HmacAuthenticationHandler(IMemoryCache memoryCache, IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock,
        IOptions<ApplicationSettingsModel> applicationSettings)
        : base(options, logger, encoder, clock)
    {
        this.applicationSettings = applicationSettings;
        this.memoryCache = memoryCache;
        if (this.allowedAps.Count == 0)
        {
            var appId = this.applicationSettings.Value.AppId;
            var apiKey = this.applicationSettings.Value.ApiKey;

            this.allowedAps.Add(appId, apiKey);
        }
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var headers = Context.Request.Headers;
        var authorizations = headers["Authorization"];

        if (authorizations.Any())
        {
            var authorization = authorizations.First();

            if (authorization.StartsWith(authenticationScheme))
            {
                var authorizationHeaderArray = this.GetAutherizationHeaderValues(authorization);

                if (authorizationHeaderArray != null)
                {
                    var appId = authorizationHeaderArray[0];
                    var incomingBase64Signature = authorizationHeaderArray[1];
                    var nonce = authorizationHeaderArray[2];
                    var requestTimeStamp = authorizationHeaderArray[3];

                    var request = Context.Request;
                    var isValid = await this.IsValidRequest(request, appId, incomingBase64Signature, nonce, requestTimeStamp);

                    if (isValid)
                    {
                        var identity = new ClaimsIdentity("api");
                        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), null,
                            "api");
                        return AuthenticateResult.Success(ticket);
                    }

                    return AuthenticateResult.Fail("Incorrect 'Authorization' header.");
                }
            }
        }

        return AuthenticateResult.Fail("Missing or malformed 'Authorization' header.");
    }

    private async Task<bool> IsValidRequest(HttpRequest request, string appId, string incomingBase64Signature, string nonce,
        string requestTimeStamp)
    {
        var requestContentBase64String = "";
        var url = request.Scheme + "://" + request.Host + request.Path;
        var requestUri = HttpUtility.UrlEncode(url.ToLower());
        var requestHttpMethod = request.Method;

        if (!this.allowedAps.ContainsKey(appId))
        {
            return false;
        }

        if (IsReplayRequest(nonce, requestTimeStamp))
        {
            return false;
        }

        var sharedKey = this.allowedAps[appId];
        request.EnableBuffering();
        var hash = await this.ComputeHash(request.Body);
        request.Body.Position = 0;

        if (hash != null)
        {
            requestContentBase64String = Convert.ToBase64String(hash);
        }

        var data = string.Format("{0}{1}{2}{3}{4}{5}", appId, requestHttpMethod, requestUri, requestTimeStamp, nonce, requestContentBase64String);

        var secretKeyBytes = Convert.FromBase64String(sharedKey);

        byte[] signature = Encoding.UTF8.GetBytes(data);

        using (var hmac = new HMACSHA256(secretKeyBytes))
        {
            byte[] signatureBytes = hmac.ComputeHash(signature);
            var convertedBytes = Convert.ToBase64String(signatureBytes);
            var result = incomingBase64Signature.Equals(convertedBytes, StringComparison.Ordinal);
            return result;
        }
    }

    private async Task<byte[]> ComputeHash(Stream requestBody)
    {
        using (var md5 = MD5.Create())
        {
            var content = await this.GetBytes(requestBody);

            byte[] hash = content.Length != 0
                ? md5.ComputeHash(content)
                : null;

            return hash;
        }
    }

    private async Task<byte[]> GetBytes(Stream input)
    {
        byte[] buffer = new byte[16 * 1024];
        using (var ms = new MemoryStream())
        {
            int read;
            while ((read = await input.ReadAsync(buffer, 0, buffer.Length)) > 0)
            {
                ms.Write(buffer, 0, read);
            }

            return ms.ToArray();
        }
    }

    private bool IsReplayRequest(string nonce, string requestTimeStamp)
    {
        if (this.memoryCache.TryGetValue(nonce, out object _))
        {
            return true;
        }

        var epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
        var currentTimeSpan = DateTime.UtcNow - epochStart;

        var serverTotalSeconds = Convert.ToUInt64(currentTimeSpan.TotalSeconds);
        var requestTotalSeconds = Convert.ToUInt64(requestTimeStamp);

        if ((serverTotalSeconds - requestTotalSeconds) > requestMaxAgeInSeconds)
        {
            return true;
        }

        this.memoryCache.Set(nonce, requestTimeStamp, DateTimeOffset.UtcNow.AddSeconds(requestMaxAgeInSeconds));

        return false;
    }

    private string[] GetAutherizationHeaderValues(string rawAuthHeader)
    {
        var credArray = rawAuthHeader.Split(' ')[1].Split(':');

        return credArray.Length == 4 ? credArray : null;
    }
}