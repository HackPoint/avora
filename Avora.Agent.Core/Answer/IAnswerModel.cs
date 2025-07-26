using Avora.Agent.Core.Query;
using Avora.Agent.Core.Storage;
using Avora.Agent.Core.Trace;

namespace Avora.Agent.Core.Answer;

public interface IAnswerModel {
    Task<string> CompleteAsync(string prompt, CancellationToken ct = default);
}

public sealed record AnswerResult(
    string Answer,
    IReadOnlyCollection<DocumentChunk> SourceChunks,
    ReasoningTrace Trace
);

public interface IAnswerGenerator {
    Task<AnswerResult> GenerateAsync(QueryContext context, List<DocumentChunk> contextChunks,
        CancellationToken ct = default);
}