using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace _21_NguyenQuangHuy_Assignment01_FrontEnd.Services;

public class BearerTokenHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BearerTokenHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token = _httpContextAccessor.HttpContext?.User.FindFirst("access_token")?.Value;
        if (!string.IsNullOrEmpty(token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
