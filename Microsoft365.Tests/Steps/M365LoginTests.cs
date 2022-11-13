using System.Collections.Generic;
using CSharpFunctionalExtensions;
using Sequence.Connectors.Microsoft365.Steps;
using Sequence.Core;
using Sequence.Core.Internal;
using Sequence.Core.Internal.Errors;
using Sequence.Core.Steps;
using Sequence.Core.Util;
using Entity = Sequence.Core.Entity;

namespace Sequence.Connectors.Microsoft365.Tests.Steps;

public partial class
    M365LoginTests : Microsoft365StepTestBase<M365Login, Unit>
{
    /// <inheritdoc />
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield break;
        }
    }

    /// <inheritdoc />
    protected override IEnumerable<ErrorCase> ErrorCases
    {
        get
        {
            yield return new ErrorCase(
                "No Settings",
                new M365Login(),
                ErrorCode.MissingStepSettings.ToErrorBuilder(
                    "Sequence.Connectors.Microsoft365"
                )
            );

            //yield return new ErrorCase(
            //    "Too many properties set",
            //    new M365Login()
            //    {
            //        HandleLogin = new LambdaFunction<Entity, Unit>(null, new DoNothing()),
            //        Token       = new SCLConstant<StringStream>("TestToken")
            //    },
            //    ErrorCode.ConflictingParameters.ToErrorBuilder("Token", "HandleLogin")
            //) { StepFactoryStoreToUse = Maybe<StepFactoryStore>.From(IntegrationTestSettingsSFS) };
        }
    }

    /// <inheritdoc />
    protected override IEnumerable<IntegrationTestCase> M365TestCases
    {
        get
        {
            yield return new IntegrationTestCase(
                "Login with token and list users",
                new M365Login() { Token = new SCLConstant<StringStream>(Token) },
                new Log()
                {
                    Value = new M365UsersRead()
                    {
                        //Stream = StaticHelpers.Constant("{\"Foo\":1}")
                    }
                }
            ) { IgnoreLoggedValues = true, };

            yield return new IntegrationTestCase(
                "Login with function and list users",
                new M365Login()
                {
                    HandleLogin = new LambdaFunction<Entity, Unit>(
                        VariableName.Item,
                        new Log()
                        {
                            Value = new EntityFormat
                            {
                                Entity = new OneOfStep<Entity, Array<Entity>>(
                                    new GetVariable<Entity>()
                                    {
                                        Variable = VariableName.Item
                                    }
                                )
                            }
                        }
                    )
                },
                new Log()
                {
                    Value = new M365UsersRead()
                    {
                        //Stream = StaticHelpers.Constant("{\"Foo\":1}")
                    }
                }
            ) { IgnoreLoggedValues = true, };
        }
    }
}
