using System.Collections.Generic;

namespace Reductech.Sequence.Connectors.Microsoft365.Steps;

/// <summary>
/// Read a list of users from Microsoft 365
/// Uses /users endpoint
/// </summary>
public sealed class M365UsersRead : CompoundStep<Array<Entity>>
{
    /// <inheritdoc />
    public override IEnumerable<Requirement> RuntimeRequirements
    {
        get
        {
            yield return new GraphScopeRequirement("User.ReadBasic.All");
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

        var connection = await stateMonad.GetOrCreateGraphConnection(
            this,
            SettingsHelpers.DefaultInitGraph,
            cancellationToken
        );

        if (connection.IsFailure)
            return connection.ConvertFailure<Array<Entity>>();

        var mailResults = await
            connection.Value.GraphServiceClient.Users
                .Request()
                .Select(
                    m => new
                    {
                        // Only request specific properties
                        m.DisplayName,
                        //m.UserPrincipalName,m.AboutMe, m.AgeGroup,
                    }
                )
                // Get at most 25 results
                .Top(take)
                // Sort by received time, newest first
                .OrderBy("DisplayName ASC")
                .GetAsync(cancellationToken);

        var entities =
            mailResults.Select(x => x.ToEntity())
                .ToSCLArray();

        return entities;
    }

    /// <summary>
    /// The number of results to take at once.  At most 50.
    /// </summary>
    [StepProperty(1)]
    [DefaultValueExplanation("25")]
    public IStep<SCLInt> Take { get; set; } = new SCLConstant<SCLInt>(new SCLInt(25));

    /// <inheritdoc />
    public override IStepFactory StepFactory { get; } =
        new SimpleStepFactory<M365UsersRead, Array<Entity>>();
}
