using System.Collections.Generic;
using Sequence.ConnectorManagement.Base;

namespace Sequence.Connectors.Microsoft365;

/// <summary>
/// This step requires a particular Graph Score
/// </summary>
public record GraphScopeRequirement(string Scope) : Requirement(
    typeof(GraphScopeRequirement).Assembly.GetName().Name!
)
{
    /// <inheritdoc />
    protected override Result<Unit, IErrorBuilder> Check(ConnectorSettings connectorSettings)
    {
        var scopes = connectorSettings.Settings?.TryGetValue(
            nameof(GraphSettings.GraphUserScopes),
            out var s
        ) == true
            ? s
            : null;

        if (scopes is IReadOnlyList<object> l
         && l.OfType<string>().Contains(Scope, StringComparer.OrdinalIgnoreCase))
        {
            return Unit.Default;
        }

        return ErrorCode.RequirementNotMet.ToErrorBuilder(GetText());
    }

    /// <inheritdoc />
    public override string GetText()
    {
        return $"Microsoft 365 Scope: '{Scope}'";
    }
}
