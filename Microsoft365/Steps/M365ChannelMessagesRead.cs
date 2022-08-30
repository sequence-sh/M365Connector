using System.Collections.Generic;

namespace Reductech.Sequence.Connectors.Microsoft365.Steps;

/// <summary>
/// Reads M365 Channels
/// </summary>
public sealed class M365ChannelMessagesRead : CompoundStep<Array<Entity>>
{
    /// <inheritdoc />
    public override IEnumerable<Requirement> RuntimeRequirements
    {
        get
        {
            yield return new GraphScopeRequirement("ChannelMessage.Read.All");
        }
    }

    /// <inheritdoc />
    protected override async Task<Result<Array<Entity>, IError>> Run(
        IStateMonad stateMonad,
        CancellationToken cancellationToken)
    {
        var data = await stateMonad.RunStepsAsync(
            TeamId.WrapNullable(StepMaps.String()),
            TeamName.WrapNullable(StepMaps.String()),
            ChannelId.WrapNullable(StepMaps.String()),
            ChannelName.WrapNullable(StepMaps.String()),
            cancellationToken
        );

        if (data.IsFailure)
            return data.ConvertFailure<Array<Entity>>();

        var (teamId, teamName, channelId, channelName) = data.Value;

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

        if (channelId.HasNoValue)
        {
            if (channelName.HasNoValue)
            {
                return ErrorCode.MissingParameter.ToErrorBuilder(nameof(ChannelId))
                    .WithLocationSingle(this);
            }

            var channelResults = await connection.Value.GraphServiceClient
                .Teams[teamId.Value]
                .Channels.Request()
                .GetAsync(cancellationToken);

            var channel = channelResults.FirstOrDefault(
                x => x.DisplayName.Equals(channelName.Value, StringComparison.OrdinalIgnoreCase)
            );

            if (channel is not null)
            {
                channelId = channel.Id;
            }
            else
            {
                return Array<Entity>.Empty;
            }
        }

        var channelMessages = await connection.Value.GraphServiceClient
            .Teams[teamId.Value]
            .Channels[channelId.Value]
            .Messages
            .Request()
            .GetAsync(cancellationToken);

        var entities = channelMessages.Select(x => x.ToEntity()).ToSCLArray();

        return entities;
    }

    /// <summary>
    /// The Id of the Team the channel belongs to.
    /// Either this or TeamName must be set.
    /// </summary>
    [StepProperty()]
    [DefaultValueExplanation("TeamName must be set.")]
    public IStep<StringStream>? TeamId { get; set; } = null!;

    /// <summary>
    /// The Name of the Team the channel belongs to.
    /// Either this or TeamId must be set.
    /// </summary>
    [StepProperty()]
    [DefaultValueExplanation("TeamId must be set.")]
    public IStep<StringStream>? TeamName { get; set; } = null!;

    /// <summary>
    /// The Id of the Channel.
    /// Either this or ChannelName must be set.
    /// </summary>
    [StepProperty()]
    [DefaultValueExplanation("ChannelName must be set.")]
    public IStep<StringStream>? ChannelId { get; set; } = null!;

    /// <summary>
    /// The Name of the Channel.
    /// Either this or ChannelId must be set.
    /// </summary>
    [StepProperty()]
    [DefaultValueExplanation("ChannelId must be set.")]
    public IStep<StringStream>? ChannelName { get; set; } = null!;

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

        if (ChannelId is null)
        {
            if (ChannelName is null)
            {
                return ErrorCode.MissingParameter.ToErrorBuilder(nameof(ChannelId))
                    .WithLocationSingle(this);
            }
        }
        else if (ChannelName is not null)
        {
            return ErrorCode.ConflictingParameters
                .ToErrorBuilder(nameof(ChannelId), nameof(ChannelName))
                .WithLocationSingle(this);
        }

        return base.VerifyThis(stepFactoryStore);
    }

    /// <inheritdoc />
    public override IStepFactory StepFactory { get; } =
        new SimpleStepFactory<M365ChannelMessagesRead, Array<Entity>>();
}
