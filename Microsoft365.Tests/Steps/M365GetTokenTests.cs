using System.Collections.Generic;
using Sequence.Connectors.Microsoft365.Steps;
using Sequence.Core;
using Sequence.Core.Steps;

namespace Sequence.Connectors.Microsoft365.Tests.Steps;

public partial class M365GetTokenTests : Microsoft365StepTestBase<M365GetToken, StringStream>
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
                "Get Token",
                new Log() { Value = new M365GetToken() }
            ) { IgnoreLoggedValues = true };
        }
    }
}
