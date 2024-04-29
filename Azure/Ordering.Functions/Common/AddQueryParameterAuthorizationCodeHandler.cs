using System.Web;

namespace Ordering.Functions.Common;

public class AddQueryParameterAuthorizationCodeHandler(string code) : DelegatingHandler
{
    private readonly string _code = code;

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var uriBuilder = new UriBuilder(request.RequestUri!);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["code"] = _code;
        uriBuilder.Query = query.ToString();
        request.RequestUri = uriBuilder.Uri;
        return base.SendAsync(request, cancellationToken);
    }
}