using System.Collections.Generic;

namespace Reductech.Sequence.Connectors.Microsoft365.Steps;

/// <summary>
/// Read a list of users from Microsoft 365
/// </summary>
public class M365UsersRead : CompoundStep<Array<Entity>>
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
        var connection = await stateMonad.GetOrCreateGraphConnection(this, cancellationToken);

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
                .Top(25)
                // Sort by received time, newest first
                .OrderBy("DisplayName ASC")
                .GetAsync(cancellationToken);

        var entities =
            mailResults.Select(
                    x => Entity.Create(
                        (nameof(User.DisplayName), x.DisplayName)
                        //(nameof(User.UserPrincipalName), x.UserPrincipalName),
                        //(nameof(User.AboutMe), x.AboutMe),
                        //(nameof(User.AgeGroup), x.AgeGroup)
                    )
                )
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
        new SimpleStepFactory<M365UsersRead, Array<Entity>>();
}
