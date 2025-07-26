namespace Avora.Agent.Infrastructure.Qdrant.Options;

public class QdrantOptions {
    public string Host { get; set; } = "localhost";
    public bool UseHttps { get; set; } = false;
    public string ApiKey { get; set; } = "";
    public string Collection { get; set; } = "avora";
}