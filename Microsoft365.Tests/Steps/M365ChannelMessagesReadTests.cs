using System.Collections.Generic;
using Reductech.Sequence.Connectors.Microsoft365.Steps;
using Reductech.Sequence.Core;
using Reductech.Sequence.Core.Internal;
using Reductech.Sequence.Core.Steps;

namespace Reductech.Sequence.Connectors.Microsoft365.Tests.Steps;

public partial class
    M365ChannelMessagesReadTests
    : Microsoft365StepTestBase<M365ChannelMessagesRead, Array<Entity>>
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
                "With Team Id and Channel Id",
                new M365Login() { Token = new SCLConstant<StringStream>(Token) },
                new Log()
                {
                    Value =
                        new EntityFormat()
                        {
                            Entity = new OneOfStep<Entity, Array<Entity>>(
                                new M365ChannelMessagesRead()
                                {
                                    TeamId = new SCLConstant<StringStream>(
                                        "d7a80c12-28e8-4e58-8b0a-c834d6cd8e2b"
                                    ),
                                    ChannelId = new SCLConstant<StringStream>(
                                        "19:RXhlpXokHMf9ur7FVWWaoQcZRNr-W_kzbkHC-zkAziY1@thread.tacv2"
                                    )
                                }
                            )
                        }
                }
            ) { IgnoreLoggedValues = true, };

            yield return new IntegrationTestCase(
                "With Team Name and Channel Name",
                new M365Login() { Token = new SCLConstant<StringStream>(Token) },
                new Log()
                {
                    Value =
                        new EntityFormat()
                        {
                            Entity = new OneOfStep<Entity, Array<Entity>>(
                                new M365ChannelMessagesRead()
                                {
                                    TeamName =
                                        new SCLConstant<StringStream>("Forensync-Demo"),
                                    ChannelName = new SCLConstant<StringStream>("General")
                                }
                            )
                        }
                }
            ) { IgnoreLoggedValues = true, };
        }
    }
}
