
using Microsoft.AspNetCore.Mvc;
using System.Net;

public static class HTTP_Context_Extensions
{
    /// <summary>
    /// Gets the IP Adress from the Current HttpRequest
    /// </summary>
    /// <param name="context">Http Request</param>
    /// <param name="ServerBehindCloudflare">Indicates if you use Cloudflare to Protect your Server it Proxys every Incomming Connection</param>
    /// <returns></returns>
    public static string GetRemoteIPAddress(this HttpContext context, bool ServerBehindCloudflare = true)
    {
        if (ServerBehindCloudflare)
        {
#pragma warning disable CS8600
#pragma warning disable CS8602
            string header = context.Request.Headers["CF-Connecting-IP"].FirstOrDefault() ?? context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (IPAddress.TryParse(header, out IPAddress ip))
            {
                return ip.MapToIPv4().ToString();
            }
        }
        return context.Connection.RemoteIpAddress.MapToIPv4().ToString();
#pragma warning restore CS8600
#pragma warning restore CS8602
    }
    /// <summary>
    /// Esay to Use HTTP ContentResult
    /// </summary>
    /// <param name="code">HTTP Status Code</param>
    /// <param name="Message">Return Message</param>
    /// <returns></returns>
    public static ContentResult HttpResponse(this HttpContext context, int code, string Message)
    {
        return new ContentResult()
        {
            StatusCode = code,
            Content = Message
        };
    }
    public static ContentResult InvalidResponse(this HttpContext context)
    {
        return new ContentResult()
        {
            StatusCode = 401,
            Content = "Invalid Request!"
        };
    }
}