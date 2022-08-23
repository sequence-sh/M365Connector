using System.Collections.Generic;

namespace Reductech.Sequence.Connectors.Microsoft365.Steps;

/// <summary>
/// Read a list of users from Microsoft 365
/// </summary>
public sealed class M365TeamsRead : CompoundStep<Array<Entity>>
{
    /// <inheritdoc />
    public override IEnumerable<Requirement> RuntimeRequirements
    {
        get
        {
            yield return new GraphScopeRequirement("Team.ReadBasic.All");
        }
    }

    /// <inheritdoc />
    protected override async Task<Result<Array<Entity>, IError>> Run(
        IStateMonad stateMonad,
        CancellationToken cancellationToken)
    {
        var connection = await stateMonad.GetOrCreateGraphConnection(this, cancellationToken);

        if (connection.IsFailure)
            return connection.ConvertFailure<Array<Entity>>();

        var teamResults = await
            connection.Value.GraphServiceClient
                .Me.JoinedTeams
                .Request()
                .Select(
                    m => new
                    {
                        // Only request specific properties
                        m.DisplayName,
                        m.Description,
                        m.Id,
                        m.Members,
                        m.Channels
                    }
                )
                //.Top(take) TOP is not allowed apparently
                .GetAsync(cancellationToken);

        var entities =
            teamResults.Select(x => x.ToEntity())
                .ToSCLArray();

        return entities;
    }

    ///// <summary>
    ///// The number of results to take at once. At most 50.
    ///// </summary>
    //[StepProperty(1)]
    //[DefaultValueExplanation("25")]
    //public IStep<SCLInt> Take { get; set; } = new SCLConstant<SCLInt>(new SCLInt(25));

    /// <inheritdoc />
    public override IStepFactory StepFactory { get; } =
        new SimpleStepFactory<M365TeamsRead, Array<Entity>>();
}
