using System.Net;

namespace InfrastructureTests.Qdrant;

public class MockHttpMessageHandler : HttpMessageHandler {
    public HttpRequestMessage? LastRequest { get; private set; }
    public string? LastContent { get; private set; }
    private HttpResponseMessage _response = new(HttpStatusCode.OK);

    public void SetJsonResponse(string json) {
        _response = new HttpResponseMessage(HttpStatusCode.OK) {
            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
        };
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken) {
        LastRequest = request;
        if (request.Content != null)
            LastContent = request.Content.ReadAsStringAsync().Result;

        return Task.FromResult(_response);
    }
}