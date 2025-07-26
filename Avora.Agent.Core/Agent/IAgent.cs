namespace Avora.Agent.Core.Agent;

/// <summary>
/// Represents the main AI Agent that generates responses based on query and retrieved context.
/// </summary>
public interface IAgent {
    /// <summary>
    /// Executes reasoning over a query and produces a structured response.
    /// </summary>
    AgentAnswer Execute(Query.Query query);
}