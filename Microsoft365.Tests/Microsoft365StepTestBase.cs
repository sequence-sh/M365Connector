using System.Collections.Generic;
using Reductech.Sequence.Core.Internal;
using Reductech.Sequence.Core.TestHarness;

namespace Reductech.Sequence.Connectors.Microsoft365.Tests;

public abstract partial class
    Microsoft365StepTestBase<TStep, TOutput> : StepTestBase<TStep, TOutput>
    where TStep : class, ICompoundStep<TOutput>, new()
    where TOutput : ISCLObject
{
    const string TenantId = "619bb56e-b872-40de-aa44-d5123e737a9a";
    const string ClientId = "b1ebd1c5-f502-4b24-b595-1e4d0c9c16d8";

    protected const string Token =
        @"abc";

    public GraphSettings UnitTestSettings { get; }
        = new(
            TenantId,
            ClientId,
            new[]
            {
                //"user.read",
                "mail.read",
                //"mail.send",
                "user.ReadBasic.all", "Team.ReadBasic.all", "Chat.Read", "Channel.ReadBasic.All",
                "ChannelMessage.Read.All"
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
