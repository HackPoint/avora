using Avora.Agent.Core.RAG;

namespace Avora.Agent.Core.Agent;

/// <summary>
/// The core AI agent that uses RAG to provide mentored, structured responses.
/// </summary>
public class ReasoningAgent(IRagPipeline pipeline, ITextGenerator generator) : IAgent {
    public AgentAnswer Execute(Query.Query query) {
        var chunks = pipeline.Run(query, 5);
        var context = string.Join("\n", chunks.Select(c => c.Content));

        var response = generator.Generate(query.OriginalText, context);

        return new AgentAnswer {
            AnswerText = response.Answer,
            Sources = chunks.Select(c => c.Source ?? "unknown").Distinct().ToArray(),
            Highlights = response.Highlights,
            SuggestedFollowUps = response.FollowUps,
            Tone = response.Tone,
            Topic = response.Topic
        };
    }
}