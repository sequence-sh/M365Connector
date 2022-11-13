namespace Sequence.Connectors.Microsoft365.Steps;

/// <summary>
/// Gets the M365 Token that was used to login.
/// </summary>
public sealed class M365GetToken : CompoundStep<StringStream>
{
    /// <inheritdoc />
    protected override async ValueTask<Result<StringStream, IError>> Run(
        IStateMonad stateMonad,
        CancellationToken cancellationToken)
    {
        var connection = await stateMonad.GetOrCreateGraphConnection(
            this,
            SettingsHelpers.DefaultInitGraph,
            cancellationToken
        );

        if (connection.IsFailure)
            return connection.ConvertFailure<StringStream>();

        return new StringStream(connection.Value.AccessToken.Token);
    }

    /// <inheritdoc />
    public override IStepFactory StepFactory { get; }
        = new SimpleStepFactory<M365GetToken, StringStream>();
}
