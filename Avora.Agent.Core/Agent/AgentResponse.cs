namespace Avora.Agent.Core.Agent;

/// <summary>
/// Internal result used by the text generation layer.
/// </summary>
public class AgentResponse {
    public required string Answer { get; init; }
    public string[] Highlights { get; init; } = [];
    public string[] FollowUps { get; init; } = [];
    public string Tone { get; init; } = "Mentor";
    public string Topic { get; init; } = string.Empty;
}