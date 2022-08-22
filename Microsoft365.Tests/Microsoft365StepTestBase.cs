using System.Collections.Generic;
using Reductech.Sequence.Core.Internal;
using Reductech.Sequence.Core.TestHarness;

namespace Reductech.Sequence.Connectors.Microsoft365.Tests;

public abstract partial class
    Microsoft365StepTestBase<TStep, TOutput> : StepTestBase<TStep, TOutput>
    where TStep : class, ICompoundStep<TOutput>, new()
    where TOutput : ISCLObject
{
    const string TenantId = "d994faac-8ca0-4e79-9892-49bccf16bb6d";
    const string ClientId = "2e76c99d-733b-45cb-81f9-b61ea87f6389";

    public GraphSettings UnitTestSettings { get; }
        = new(
            TenantId,
            ClientId,
            new[]
            {
                //"user.read",
                "mail.read",
                //"mail.send",
                "user.ReadBasic.all", "Team.ReadBasic.all", "Chat.Read",
            }
        );

    public StepFactoryStore IntegrationTestSettingsSFS => SettingsHelpers.CreateStepFactoryStore(
        UnitTestSettings,
        typeof(GraphSettings).Assembly
    );

    /// <inheritdoc />
    protected override IEnumerable<ErrorCase> ErrorCases
    {
        get
        {
            foreach (var baseErrorCase in base.ErrorCases)
            {
                var caseWithSettings =
                    baseErrorCase.WithStepFactoryStore(IntegrationTestSettingsSFS);

                yield return caseWithSettings;
            }
        }
    }

    protected abstract IEnumerable<IntegrationTestCase> M365TestCases
    {
        get;
    }
}
