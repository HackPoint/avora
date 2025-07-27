using Avora.Agent.Core;
using Avora.Agent.Core.Agent;
using Avora.Agent.Core.Builders;
using Avora.Agent.Core.Embeddings;
using Avora.Agent.Core.Indexing;
using Avora.Agent.Core.Query;
using Avora.Agent.Core.RAG;
using Avora.Agent.Core.Storage;
using Avora.Agent.Core.Tokens;
using Avora.Agent.Infrastructure.Embeddings;
using Avora.Agent.Infrastructure.Embeddings.Options;
using Avora.Agent.Infrastructure.Generation;
using Avora.Agent.Infrastructure.Qdrant;
using Avora.Agent.Infrastructure.Qdrant.Options;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var config = builder.Configuration;

// === Options ===
services.Configure<QdrantOptions>(config.GetSection("Qdrant"));
services.Configure<FastApiOptions>(config.GetSection("FastApi"));

// === Http Clients ===
services.AddHttpClient("qdrant", (sp, client) => {
    var options = sp.GetRequiredService<IOptions<QdrantOptions>>().Value;
    client.BaseAddress = new Uri($"{(options.UseHttps ? "https" : "http")}://{options.Host}");
    client.DefaultRequestHeaders.Add("api-key", options.ApiKey);
});

services.AddHttpClient("fastapi", (sp, client) => {
    var options = sp.GetRequiredService<IOptions<FastApiOptions>>().Value;
    client.BaseAddress = new Uri(options.Host);
});

// === Infrastructure Services ===
services.AddSingleton<QdrantClient>(sp => {
    var http = sp.GetRequiredService<IHttpClientFactory>().CreateClient("qdrant");
    var options = sp.GetRequiredService<IOptions<QdrantOptions>>().Value;
    var client = new QdrantClient(http, options.Collection);
    _ = client.EnsureCollectionAsync();
    return client;
});

services.AddSingleton<IEmbeddingModel>(sp => {
    var http = sp.GetRequiredService<IHttpClientFactory>().CreateClient("fastapi");
    return new HttpEmbeddingModel(http);
});

services.AddSingleton<ITextGenerator>(sp => {
    var http = sp.GetRequiredService<IHttpClientFactory>().CreateClient("fastapi");
    return new FastApiTextGenerator(http);
});


services.AddSingleton<IVectorIndex, QdrantVectorIndex>();
services.AddSingleton<IDocumentStore, QdrantDocumentStore>();
services.AddSingleton<ITokenizer, Tokenizer>();
services.AddSingleton<IQueryFactory, QueryFactory>();

// === Core Pipeline + Agent ===
services.AddSingleton<IRagPipeline, RagPipeline>();
services.AddSingleton<IAgent, ReasoningAgent>();
builder.Services.AddSingleton<IAgent>(sp => {
    var tokenizer = sp.GetRequiredService<ITokenizer>();
    var embedding = sp.GetRequiredService<IEmbeddingModel>();
    var generator = sp.GetRequiredService<ITextGenerator>();
    var index = sp.GetRequiredService<IVectorIndex>();
    var store = sp.GetRequiredService<IDocumentStore>();

    return new ReasoningAgentBuilder()
        .WithTokenizer(tokenizer)
        .WithEmbeddingModel(embedding)
        .WithVectorIndex(index)
        .WithDocumentStore(store)
        .WithTextGenerator(generator)
        .Build();
});

// === API Setup ===
services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.MapOpenApi();
}

app.UseAuthorization();
app.MapControllers();
app.Run();