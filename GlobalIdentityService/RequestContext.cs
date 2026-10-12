using System.Net;

namespace GlobalIdentityService
{
    public class RequestContext(IHttpContextAccessor accessor)
    {
        public IPAddress? Ip => accessor.HttpContext?.Connection.RemoteIpAddress;
        public string? UserAgent => accessor.HttpContext?.Request.Headers.UserAgent.ToString();
        public string? TraceId => accessor.HttpContext?.TraceIdentifier;
        public string? Path => accessor.HttpContext?.Request.Path;
    }
}
