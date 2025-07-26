namespace Avora.Agent.Api.Models;

public class DocumentChunkDto {
    public string Content { get; set; } = default!;
    public string? Source { get; set; }
}