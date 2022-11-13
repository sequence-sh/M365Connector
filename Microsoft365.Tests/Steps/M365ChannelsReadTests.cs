using System.Collections.Generic;
using Sequence.Connectors.Microsoft365.Steps;
using Sequence.Core;
using Sequence.Core.Internal;
using Sequence.Core.Steps;

namespace Sequence.Connectors.Microsoft365.Tests.Steps;

public partial class
    M365ChannelsReadTests
    : Microsoft365StepTestBase<M365ChannelsRead, Array<Entity>>
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
                "Team Id",
                new M365Login() { Token = new SCLConstant<StringStream>(Token) },
                new Log()
                {
                    Value =
                        new EntityFormat()
                        {
                            Entity = new OneOfStep<Entity, Array<Entity>>(
                                new M365ChannelsRead()
                                {
                                    TeamId = new SCLConstant<StringStream>(
                                        "d7a80c12-28e8-4e58-8b0a-c834d6cd8e2b"
                                    ),
                                }
                            )
                        }
                }
            ) { IgnoreLoggedValues = true, };

            yield return new IntegrationTestCase(
                "Team Name",
                new M365Login() { Token = new SCLConstant<StringStream>(Token) },
                new Log()
                {
                    Value =
                        new EntityFormat()
                        {
                            Entity = new OneOfStep<Entity, Array<Entity>>(
                                new M365ChannelsRead()
                                {
                                    TeamName =
                                        new SCLConstant<StringStream>("Forensync-Demo"),
                                }
                            )
                        }
                }
            ) { IgnoreLoggedValues = true, };
        }
    }
}
