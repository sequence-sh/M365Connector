using System.Collections.Generic;
using Sequence.Connectors.Microsoft365.Steps;
using Sequence.Core;
using Sequence.Core.Internal;
using Sequence.Core.Steps;

namespace Sequence.Connectors.Microsoft365.Tests.Steps;

public partial class
    M365ChatsReadTests : Microsoft365StepTestBase<M365ChatsRead, Array<Entity>>
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
    protected override IEnumerable<IntegrationTestCase> M365TestCases
    {
        get
        {
            yield return new IntegrationTestCase(
                "Single Property",
                new M365Login() { Token = new SCLConstant<StringStream>(Token) },
                new Log()
                {
                    Value = new EntityFormat()
                    {
                        Entity = new OneOfStep<Entity, Array<Entity>>(
                            new M365ChatsRead()
                            {
                                Take = new SCLConstant<SCLInt>(new SCLInt(50))
                                //Stream = StaticHelpers.Constant("{\"Foo\":1}")
                            }
                        )
                    }
                }
            ) { IgnoreLoggedValues = true, };
        }
    }
}
