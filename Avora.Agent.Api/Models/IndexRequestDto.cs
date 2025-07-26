namespace Avora.Agent.Api.Models;

public class IndexRequestDto {
    public string OriginalText { get; set; } = default!;
    public List<DocumentChunkDto> Chunks { get; set; } = new();
}