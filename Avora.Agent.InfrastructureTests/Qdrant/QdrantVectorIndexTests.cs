using System.Net;
using System.Text.Json;
using Avora.Agent.Core.Query;
using Avora.Agent.Infrastructure.Qdrant;
using FluentAssertions;
using Moq;
using Moq.Protected;

namespace InfrastructureTests.Qdrant;

public class QdrantVectorIndexTests {
    private static HttpClient CreateMockHttpClient(Func<HttpRequestMessage, HttpResponseMessage> handlerFunc) {
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);

        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync((HttpRequestMessage request, CancellationToken _) => handlerFunc(request));

        return new HttpClient(handlerMock.Object) {
            BaseAddress = new Uri("http://localhost:6333")
        };
    }

    [Fact]
    public async Task Upsert_Should_Send_Correct_Request() {
        var client = CreateMockHttpClient(req => {
            req.Method.Should().Be(HttpMethod.Put);
            req.RequestUri!.AbsolutePath.Should().Contain("points");

            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        var qdrant = new QdrantClient(client, "test_collection");
        await qdrant.UpsertAsync("guid-1", [0.1f, 0.2f]);
    }

    [Fact]
    public async Task Delete_Should_Send_Correct_Body() {
        var client = CreateMockHttpClient(req => {
            req.Method.Should().Be(HttpMethod.Post);
            req.RequestUri!.AbsolutePath.Should().Contain("delete");

            var body = JsonSerializer.Deserialize<JsonElement>(req.Content!.ReadAsStringAsync().Result);
            body.GetProperty("points")[0].GetString().Should().Be("guid-1");

            return new HttpResponseMessage(HttpStatusCode.OK);
        });

        var qdrant = new QdrantClient(client, "test_collection");
        await qdrant.DeleteAsync("guid-1");
    }

    [Fact]
    public async Task Search_Should_Parse_Results() {
        var mockResponse = new {
            result = new[] {
                new { id = "abc", score = 0.95f },
                new { id = "xyz", score = 0.82f }
            }
        };

        var client = CreateMockHttpClient(req => {
            req.Method.Should().Be(HttpMethod.Post);
            req.RequestUri!.AbsolutePath.Should().Contain("search");

            var json = JsonSerializer.Serialize(mockResponse);
            return new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json")
            };
        });

        var qdrant = new QdrantClient(client, "test_collection");
        var results = await qdrant.SearchAsync([0.1f, 0.3f], 2);

        results.Should().HaveCount(2);
        results[0].Id.Should().Be("abc");
        results[0].Score.Should().BeApproximately(0.95f, 0.001f);
    }
    
    [Fact]
    public void QueryId_Should_Convert_From_And_To_String() {
        var guid = Guid.NewGuid();
        var id = QueryId.From(guid);
        id.ToGuid().Should().Be(guid);
        id.ToString().Should().Be(guid.ToString());

        var parsed = QueryId.From(guid.ToString());
        parsed.ToGuid().Should().Be(guid);
    }
}