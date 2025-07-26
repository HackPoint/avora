using System.Net.Http.Json;
using Avora.Agent.Core.Agent;

namespace Avora.Agent.Infrastructure.Generation;

public class FastApiTextGenerator(HttpClient http) : ITextGenerator {
    public async Task<AgentResponse> GenerateAsync(string question, string context) {
        var payload = new { question, context };

        var response = await http.PostAsJsonAsync("/generate", payload);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<AgentResponse>();
        return result ?? throw new InvalidOperationException("No response from text generator");
    }

    // Sync wrapper (optional)
    public AgentResponse Generate(string question, string context) =>
        GenerateAsync(question, context).GetAwaiter().GetResult();
}