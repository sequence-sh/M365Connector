﻿using System.Collections.Generic;

namespace Sequence.Connectors.Microsoft365.Steps;

/// <summary>
/// Read User mail from Microsoft 365.
/// Uses /me/mailFolders('{Folder}')/messages endpoint
/// </summary>
public sealed class M365MailRead : CompoundStep<Array<Entity>>
{
    /// <inheritdoc />
    public override IEnumerable<Requirement> RuntimeRequirements
    {
        get
        {
            yield return new GraphScopeRequirement("Mail.Read");
        }
    }

    /// <inheritdoc />
    protected override async ValueTask<Result<Array<Entity>, IError>> Run(
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

        var connection = await stateMonad.GetOrCreateGraphConnection(
            this,
            SettingsHelpers.DefaultInitGraph,
            cancellationToken
        );

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
            mailResults.Select(x => x.ToEntity())
                .ToSCLArray();

        return entities;
    }

    /// <summary>
    /// The number of results to take at once. At most 50.
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
