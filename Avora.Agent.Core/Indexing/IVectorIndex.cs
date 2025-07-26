using Avora.Agent.Core.Query;

namespace Avora.Agent.Core.Indexing;


/// <summary>
/// Represents an abstract index for storing and searching vectorized queries.
/// </summary>
public interface IVectorIndex {
    void Upsert(Query.Query query, QueryId id);
    List<(QueryId Id, float Score)> Search(float[] embedding, int k = 5);
    void Delete(QueryId id);
}
