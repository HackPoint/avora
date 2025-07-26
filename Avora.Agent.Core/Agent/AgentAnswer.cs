namespace Avora.Agent.Core.Agent;


/// <summary>
/// Represents a structured, reasoned response from the AI agent.
/// </summary>
public class AgentAnswer {
    public required string AnswerText { get; init; }
    public required string[] Sources { get; init; }
    public string[] Highlights { get; init; } = [];
    public string[] SuggestedFollowUps { get; init; } = [];
    public string Tone { get; init; } = "Mentor"; // Mentor | Teacher | Interviewer
    public string Topic { get; init; } = string.Empty;
}