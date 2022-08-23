namespace Reductech.Sequence.Connectors.Microsoft365.Steps;

/// <summary>
/// Reads M365 Channels
/// </summary>
public sealed class M365ChannelsRead : CompoundStep<Array<Entity>>
{
    /// <inheritdoc />
    protected override async Task<Result<Array<Entity>, IError>> Run(
        IStateMonad stateMonad,
        CancellationToken cancellationToken)
    {
        var data = await stateMonad.RunStepsAsync(
            TeamId.WrapNullable(StepMaps.String()),
            TeamName.WrapNullable(StepMaps.String()),
            cancellationToken
        );

        if (data.IsFailure)
            return data.ConvertFailure<Array<Entity>>();

        var (teamId, teamName) = data.Value;

        var connection = await stateMonad.GetOrCreateGraphConnection(this, cancellationToken);

        if (connection.IsFailure)
            return connection.ConvertFailure<Array<Entity>>();

        if (teamId.HasNoValue)
        {
            if (teamName.HasNoValue)
            {
                return ErrorCode.MissingParameter.ToErrorBuilder(nameof(TeamId))
                    .WithLocationSingle(this);
            }

            var teamResults = await
                connection.Value.GraphServiceClient
                    .Me.JoinedTeams
                    .Request()
                    .Select(m => new { m.DisplayName, m.Id, })
                    .GetAsync(cancellationToken);

            var team = teamResults.FirstOrDefault(
                x => x.DisplayName.Equals(teamName.Value, StringComparison.OrdinalIgnoreCase)
            );

            if (team is not null)
            {
                teamId = team.Id;
            }
            else
            {
                return Array<Entity>.Empty;
            }
        }

        var allChannels = await connection.Value.GraphServiceClient
            .Teams[teamId.Value]
            .AllChannels
            .Request()
            .GetAsync(cancellationToken);

        var entities = allChannels.Select(x => x.ToEntity()).ToSCLArray();

        return entities;
    }

    /// <summary>
    /// The Id of the Team to get the Channels of.
    /// Either this or TeamName must be set.
    /// </summary>
    [StepProperty()]
    [DefaultValueExplanation("TeamName must be set.")]
    public IStep<StringStream>? TeamId { get; set; } = null!;

    /// <summary>
    /// The Name of the Team to get the Channels of.
    /// Either this or TeamId must be set.
    /// </summary>
    [StepProperty()]
    [DefaultValueExplanation("TeamId must be set.")]
    public IStep<StringStream>? TeamName { get; set; } = null!;

    /// <inheritdoc />
    public override Result<Unit, IError> VerifyThis(StepFactoryStore stepFactoryStore)
    {
        if (TeamId is null)
        {
            if (TeamName is null)
            {
                return ErrorCode.MissingParameter.ToErrorBuilder(nameof(TeamId))
                    .WithLocationSingle(this);
            }
        }
        else if (TeamName is not null)
        {
            return ErrorCode.ConflictingParameters.ToErrorBuilder(nameof(TeamId), nameof(TeamName))
                .WithLocationSingle(this);
        }

        return base.VerifyThis(stepFactoryStore);
    }

    /// <inheritdoc />
    public override IStepFactory StepFactory { get; } =
        new SimpleStepFactory<M365ChannelsRead, Array<Entity>>();
}
