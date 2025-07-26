using System.Text.Json;
using Avora.Agent.Core.Query;
using Avora.Agent.Core.Storage;
using Avora.Agent.Infrastructure.Qdrant;
using FluentAssertions;

namespace InfrastructureTests.Qdrant;

public class QdrantDocumentStoreTests {
    private readonly QueryId _id = QueryId.From(Guid.NewGuid());
    private readonly float[] _vector = new[] { 0.1f, 0.2f, 0.3f };
    private const string _collection = "test-docs";

    [Fact]
    public void Upsert_Should_Send_Valid_Http_Request() {
        // Arrange
        var handler = new MockHttpMessageHandler();
        var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost:6333") };
        var qdrant = new QdrantClient(client, _collection);
        var store = new QdrantDocumentStore(qdrant);

        var chunk = new DocumentChunk(_id, "content", _vector, "source.txt", 1);

        // Act
        store.Upsert(chunk);

        // Assert
        var request = handler.LastRequest;
        request.Should().NotBeNull();
        request.Method.Should().Be(HttpMethod.Put);
        request.RequestUri!.ToString().Should().Contain($"/collections/{_collection}/points");

        var json = JsonDocument.Parse(handler.LastContent!);
        var root = json.RootElement.GetProperty("points")[0];
        root.GetProperty("id").GetString().Should().Be(_id.ToString());
        root.GetProperty("payload").GetProperty("content").GetString().Should().Be("content");
        root.GetProperty("payload").GetProperty("source").GetString().Should().Be("source.txt");
        root.GetProperty("payload").GetProperty("index").GetInt32().Should().Be(1);
    }

    [Fact]
    public void Delete_Should_Trigger_Http_Request() {
        // Arrange
        var handler = new MockHttpMessageHandler();
        var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost:6333") };
        var qdrant = new QdrantClient(client, _collection);
        var store = new QdrantDocumentStore(qdrant);

        // Act
        store.Delete(_id);

        // Assert
        handler.LastRequest.Should().NotBeNull();
        handler.LastRequest.Method.Should().Be(HttpMethod.Post);
        handler.LastRequest.RequestUri!.ToString().Should().Contain($"/collections/{_collection}/points/delete");
    }

    [Fact]
    public void GetChunks_Should_Return_Valid_DocumentChunk()
    {
        // Arrange
        var handler = new MockHttpMessageHandler();

        var json = new {
            result = new {
                points = new[] {
                    new {
                        id = _id.ToString(),
                        vector = _vector,
                        payload = new {
                            content = "hello world",
                            source = "source.md",
                            index = 2
                        }
                    }
                }
            }
        };

        handler.SetJsonResponse(JsonSerializer.Serialize(json));

        var client = new HttpClient(handler) { BaseAddress = new Uri("http://localhost:6333") };
        var qdrant = new QdrantClient(client, _collection);
        var store = new QdrantDocumentStore(qdrant);

        // Act
        var chunks = store.GetChunks(new[] { _id });

        // Assert
        chunks.Should().HaveCount(1);
        var doc = chunks[0];
        doc.Id.Should().Be(_id);
        doc.Content.Should().Be("hello world");
        doc.Source.Should().Be("source.md");
        doc.Index.Should().Be(2);
        doc.Embedding.Should().BeEquivalentTo(_vector);
    }
}