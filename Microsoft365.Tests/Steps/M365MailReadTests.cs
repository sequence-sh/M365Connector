using System.Collections.Generic;
using Sequence.Connectors.Microsoft365.Steps;
using Sequence.Core;
using Sequence.Core.Steps;

namespace Sequence.Connectors.Microsoft365.Tests.Steps;

public partial class
    M365MailReadTests : Microsoft365StepTestBase<M365MailRead, Array<Entity>>
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
                new Log()
                {
                    Value = new M365MailRead()
                    {
                        //Stream = StaticHelpers.Constant("{\"Foo\":1}")
                    }
                }
            ) { IgnoreLoggedValues = true, };
        }
    }
}
