namespace Reductech.Sequence.Connectors.Microsoft365.Steps;

/// <summary>
/// Login to M365. You do not need to do this.
/// </summary>
public sealed class M365Login : CompoundStep<Unit>
{
    /// <inheritdoc />
    protected override async Task<Result<Unit, IError>> Run(
        IStateMonad stateMonad,
        CancellationToken cancellationToken)
    {
        if (Token is null)
        {
            var connection = await stateMonad.GetOrCreateGraphConnection(this, cancellationToken);

            if (connection.IsFailure)
                return connection.ConvertFailure<Unit>();

            return Unit.Default;
        }
        else
        {
            var data = await stateMonad.RunStepsAsync(Token.WrapStringStream(), cancellationToken);

            if (data.IsFailure)
                return data.ConvertFailure<Unit>();

            var connection = await stateMonad.CreateGraphConnection(
                this,
                data.Value,
                cancellationToken
            );

            if (connection.IsFailure)
                return connection.ConvertFailure<Unit>();

            return Unit.Default;
        }
    }

    /// <summary>
    /// The access token.
    /// If this is not set, the user must authenticate the login by going to https://login.microsoftonline.com/common/oauth2/deviceauth
    /// and entering the code which is logged.
    /// </summary>
    [StepProperty(1)]
    [DefaultValueExplanation("User must authenticate the login.")]
    public IStep<StringStream>? Token { get; set; } = null!;

    /// <inheritdoc />
    public override IStepFactory StepFactory { get; } = new SimpleStepFactory<M365Login, Unit>();
}
