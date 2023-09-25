using Microsoft.Net.Http.Headers;

namespace GameServer.Web.Extensions;

public static class HttpRequestExtensions
{
  public static bool IsTokenAuth(this HttpRequest request)
  {
    string? auth = request.Headers[HeaderNames.Authorization];
    
    return !string.IsNullOrEmpty(auth) && auth.StartsWith("Bearer ");
  }
}
