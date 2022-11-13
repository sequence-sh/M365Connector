namespace Sequence.Connectors.Microsoft365;

public sealed record GraphSettings
    (string TenantId, string ClientId, string[]? GraphUserScopes) : IEntityConvertible;
