using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Graph;
using Reductech.Sequence.Core.Internal;
using Reductech.Sequence.Core;
using Reductech.Sequence.Core.Attributes;
using Reductech.Sequence.Core.Internal.Errors;
using Reductech.Sequence.Core.Util;
using Entity = Reductech.Sequence.Core.Entity;

namespace Reductech.Sequence.Connectors.Microsoft365.Steps;

public class M365MailRead : CompoundStep<Array<Entity>>
{
    /// <inheritdoc />
    protected override async Task<CSharpFunctionalExtensions.Result<Array<Entity>, IError>> Run(
        IStateMonad stateMonad,
        CancellationToken cancellationToken)
    {
        var data = await stateMonad.RunStepsAsync(
            Folder.WrapStringStream(),
            Take,
            cancellationToken
        );

        if (data.IsFailure)
            return data.ConvertFailure<Array<Entity>>();

        var (folder, take) = data.Value;

        var connection = await stateMonad.GetOrCreateGraphConnection(this, cancellationToken);

        if (connection.IsFailure)
            return connection.ConvertFailure<Array<Entity>>();

        var mailResults = await
            connection.Value.GraphServiceClient.Me
                .MailFolders[folder]
                .Messages
                .Request()
                .Select(
                    m => new
                    {
                        // Only request specific properties
                        m.From,
                        m.IsRead,
                        m.ReceivedDateTime,
                        m.Subject,
                        m.Body,
                    }
                )
                // Get at most 25 results
                .Top(take)
                // Sort by received time, newest first
                .OrderBy("ReceivedDateTime DESC")
                .GetAsync(cancellationToken);

        var entities =
            mailResults.Select(
                    x => Entity.Create(
                        (nameof(Message.From), x.From.EmailAddress.Name),
                        (nameof(Message.IsRead), x.IsRead),
                        (nameof(Message.ReceivedDateTime), x.ReceivedDateTime?.DateTime),
                        (nameof(Message.Subject), x.Subject),
                        (nameof(Message.Body), x.Body.Content)
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

    /// <summary>
    /// The name of the Mail Folder
    /// </summary>
    [StepProperty(2)]
    [DefaultValueExplanation("Inbox")]
    public IStep<StringStream> Folder { get; set; } = new SCLConstant<StringStream>("Inbox");

    /// <inheritdoc />
    public override IStepFactory StepFactory { get; } =
        new SimpleStepFactory<M365MailRead, Array<Entity>>();
}
