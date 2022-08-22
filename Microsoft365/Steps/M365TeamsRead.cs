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
        var data = await stateMonad.RunStepsAsync(
            Take,
            cancellationToken
        );

        if (data.IsFailure)
            return data.ConvertFailure<Array<Entity>>();

        var take = data.Value;

        var connection = await stateMonad.GetOrCreateGraphConnection(this, cancellationToken);

        if (connection.IsFailure)
            return connection.ConvertFailure<Array<Entity>>();

        var mailResults = await
            connection.Value.GraphServiceClient.Teams
                .Request()
                .Select(
                    m => new
                    {
                        // Only request specific properties
                        m.DisplayName, m.Description //, m.Members
                        //m.UserPrincipalName,m.AboutMe, m.AgeGroup,
                    }
                )
                // Get at most 25 results
                .Top(take)
                // Sort by received time, newest first
                .OrderBy("DisplayName ASC")
                .GetAsync(cancellationToken);

        //Microsoft.Graph.ServiceException
        //Code: NotFound
        //Message: Requested API is not supported. Please check the path.

        var entities =
            mailResults.Select(x => x.ToEntity())
                .ToSCLArray();

        return entities;
    }

    /// <summary>
    /// The number of results to take at once
    /// </summary>
    [StepProperty(1)]
    [DefaultValueExplanation("25")]
    public IStep<SCLInt> Take { get; set; } = new SCLConstant<SCLInt>(new SCLInt(25));

    /// <inheritdoc />
    public override IStepFactory StepFactory { get; } =
        new SimpleStepFactory<M365TeamsRead, Array<Entity>>();
}
