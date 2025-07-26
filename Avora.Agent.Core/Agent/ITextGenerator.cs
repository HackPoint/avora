namespace Avora.Agent.Core.Agent;

public interface ITextGenerator {
    /// <summary>
    /// Generates a structured response from question + context.
    /// </summary>
    AgentResponse Generate(string question, string context);
}