using System.Collections.Generic;

namespace Reductech.Sequence.Connectors.Microsoft365.Steps;

/// <summary>
/// Read User mail from Microsoft 365. 
/// </summary>
public sealed class M365ChatsRead : CompoundStep<Array<Entity>>
{
    /// <inheritdoc />
    public override IEnumerable<Requirement> RuntimeRequirements
    {
        get
        {
            yield return new GraphScopeRequirement("Chat.Read");
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

        var chatResults = await connection.Value.GraphServiceClient
            .Chats
            .Request()
            .Select(c => new { c.Topic, c.Members, c.Messages })
            .Top(take)
            //.OrderBy()
            .GetAsync(cancellationToken);

        var entities =
            chatResults.Select(x => x.ToEntity())
                .ToSCLArray();

        return entities;
    }

    /// <summary>
    /// The number of results to take at once. At most 50.
    /// </summary>
    [StepProperty(1)]
    [DefaultValueExplanation("25")]
    public IStep<SCLInt> Take { get; set; } = new SCLConstant<SCLInt>(new SCLInt(25));

    /// <inheritdoc />
    public override IStepFactory StepFactory { get; } =
        new SimpleStepFactory<M365ChatsRead, Array<Entity>>();
}
