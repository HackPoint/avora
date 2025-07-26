namespace Avora.Agent.Core.Query;

/// <summary>
/// Uniquely identifies a Query within the system.
/// </summary>
public readonly struct QueryId(Guid value) : IEquatable<QueryId> {
    public Guid Value { get; } = value;

    public static QueryId From(string id) {
        return Guid.TryParse(id, out var guid)
            ? new QueryId(guid)
            : throw new FormatException($"Invalid GUID: {id}");
    }

    public static QueryId From(Guid guid) => new QueryId(guid);

    public static QueryId New() => new(Guid.NewGuid());

    public override string ToString() => Value.ToString();

    public Guid ToGuid() => Value;
    
    public bool Equals(QueryId other) => Value.Equals(other.Value);
    public override bool Equals(object? obj) => obj is QueryId other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();

    public static bool operator ==(QueryId left, QueryId right) => left.Equals(right);
    public static bool operator !=(QueryId left, QueryId right) => !left.Equals(right);
}