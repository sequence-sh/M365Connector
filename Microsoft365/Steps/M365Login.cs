using System.Collections.Generic;
using System.Collections.Immutable;

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
        if (Token is not null)
        {
            var token = await stateMonad.RunStepsAsync(Token.WrapStringStream(), cancellationToken);

            if (token.IsFailure)
                return token.ConvertFailure<Unit>();

            var connection = await stateMonad.CreateGraphConnection(
                this,
                token.Value,
                cancellationToken
            );

            if (connection.IsFailure)
                return connection.ConvertFailure<Unit>();

            return Unit.Default;
        }
        else
        {
            Func<DeviceCodeInfo, IStateMonad, IStep, CancellationToken, Task> function;

            if (HandleLogin is null)
            {
                function = SettingsHelpers.DefaultInitGraph;
            }
            else
            {
                function = async (deviceCodeInfo, monad, _, token) =>
                {
                    var entity = Entity.Create(
                        new (EntityPropertyKey key, ISCLObject value)[]
                        {
                            new(
                                EntityPropertyKey.Create(nameof(DeviceCodeInfo.ClientId)),
                                new StringStream(deviceCodeInfo.ClientId)
                            ),
                            new(
                                EntityPropertyKey.Create(nameof(DeviceCodeInfo.DeviceCode)),
                                new StringStream(deviceCodeInfo.DeviceCode)
                            ),
                            new(
                                EntityPropertyKey.Create(nameof(DeviceCodeInfo.Message)),
                                new StringStream(deviceCodeInfo.Message)
                            ),
                            new(
                                EntityPropertyKey.Create(nameof(DeviceCodeInfo.UserCode)),
                                new StringStream(deviceCodeInfo.UserCode)
                            ),
                            new(
                                EntityPropertyKey.Create(nameof(DeviceCodeInfo.VerificationUri)),
                                new StringStream(deviceCodeInfo.VerificationUri.OriginalString)
                            ),
                            new(
                                EntityPropertyKey.Create(nameof(DeviceCodeInfo.ExpiresOn)),
                                new SCLDateTime(deviceCodeInfo.ExpiresOn.Date)
                            ),
                        }
                    );

                    var newMonad = new ScopedStateMonad(
                        monad,
                        ImmutableDictionary<VariableName, ISCLObject>.Empty,
                        HandleLogin.VariableNameOrItem,
                        new KeyValuePair<VariableName, ISCLObject>(
                            HandleLogin.VariableNameOrItem,
                            entity
                        )
                    );

                    await HandleLogin.StepTyped.Run(newMonad, token);
                };
            }

            var connection = await stateMonad.GetOrCreateGraphConnection(
                this,
                function,
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
    [StepProperty()]
    [DefaultValueExplanation("Not set")]
    public IStep<StringStream>? Token { get; set; } = null!;

    /// <summary>
    /// How to handle the login url and code.
    /// The function takes an entity with the all the relevant properties, particularly 'VerificationUri' and 'UserCode'
    /// If neither this nor the Token is set, the token and code will be logged.
    /// </summary>
    [FunctionProperty]
    [DefaultValueExplanation("Not set")]
    public LambdaFunction<Entity, Unit>? HandleLogin { get; set; } = null!;

    /// <inheritdoc />
    public override Result<Unit, IError> VerifyThis(StepFactoryStore stepFactoryStore)
    {
        if (Token is not null && HandleLogin is not null)
        {
            return ErrorCode.ConflictingParameters.ToErrorBuilder(
                    nameof(Token),
                    nameof(HandleLogin)
                )
                .WithLocationSingle(this);
        }

        return base.VerifyThis(stepFactoryStore);
    }

    /// <inheritdoc />
    public override IStepFactory StepFactory { get; } = new SimpleStepFactory<M365Login, Unit>();
}
