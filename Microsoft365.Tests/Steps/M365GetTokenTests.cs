using System.Collections.Generic;
using Reductech.Sequence.Connectors.Microsoft365.Steps;
using Reductech.Sequence.Core;
using Reductech.Sequence.Core.Steps;

namespace Reductech.Sequence.Connectors.Microsoft365.Tests.Steps;

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
