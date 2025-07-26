using Avora.Agent.Infrastructure.Qdrant.Options;
using Microsoft.Extensions.Options;

namespace Avora.Agent.Infrastructure.Qdrant;

public class QdrantClientFactory(IOptions<QdrantOptions> options) {
    private readonly QdrantOptions _options = options.Value;

    public QdrantClient Create(HttpClient httpClient)
    {
        httpClient.BaseAddress = new Uri($"{(_options.UseHttps ? "https" : "http")}://{_options.Host}");
        httpClient.DefaultRequestHeaders.Add("api-key", _options.ApiKey);

        return new QdrantClient(httpClient, _options.Collection);
    }
}