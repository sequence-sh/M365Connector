using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using AutoTheory;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Reductech.Sequence.Core;
using Reductech.Sequence.Core.Abstractions;
using Reductech.Sequence.Core.Internal;
using Reductech.Sequence.Core.Internal.Errors;
using Reductech.Sequence.Core.Internal.Parser;
using Reductech.Sequence.Core.Internal.Serialization;
using Reductech.Sequence.Core.Steps;
using Reductech.Sequence.Core.TestHarness;
using Reductech.Sequence.Core.Util;
using Xunit;
using Xunit.Abstractions;

namespace Reductech.Sequence.Connectors.Microsoft365.Tests;

[Collection("RequiresMicrosoft365Connection")]
public abstract partial class Microsoft365StepTestBase<TStep, TOutput>
{
    [GenerateAsyncTheory("Microsoft365Integration", Category = "Integration")]
    public IEnumerable<IntegrationTestCase> IntegrationTestCasesWithSettings
    {
        get
        {
            foreach (var m365TestCase in M365TestCases)
            {
                yield return new IntegrationTestCase(
                    // Name needs to have microsoft365 version in parentheses for ci script to build summary
                    $"{m365TestCase.Name}",
                    m365TestCase.Step
                ).WithStepFactoryStore(IntegrationTestSettingsSFS);
            }
        }
    }

    private static bool IsVersionCompatible(IStep step) //, Version microsoft365Version)
    {
        var settings = new GraphSettings("", "", null);

        var sfs = SettingsHelpers.CreateStepFactoryStore(
            settings,
            typeof(GraphConnection).Assembly
        );

        var r = step.Verify(sfs);
        return r.IsSuccess;
    }

    public class Microsoft365IntegrationTestCase
    {
        public Microsoft365IntegrationTestCase(string name, params IStep<Unit>[] steps)
        {
            Name = name;

            Step = new Sequence<Unit> { InitialSteps = steps, FinalStep = new DoNothing() };
        }

        public string Name { get; }
        public Sequence<Unit> Step { get; }
    }

    public record IntegrationTestCase : CaseThatExecutes
    {
        public IntegrationTestCase(string name, params IStep<Unit>[] steps) : base(
            name,
            new List<string>()
        )
        {
            Steps              = steps;
            IgnoreFinalState   = true;
            IgnoreLoggedValues = true;

            Step = new Sequence<Unit> { InitialSteps = steps, FinalStep = new DoNothing() };
        }

        /// <inheritdoc />
        public override LogLevel OutputLogLevel => OutputLogLevel1;

        public LogLevel OutputLogLevel1 { get; set; } = LogLevel.Debug;

        public Sequence<Unit> Step { get; }

        public IStep<Unit>[] Steps { get; }

        /// <inheritdoc />
        public override async Task<IStep> GetStepAsync(
            IExternalContext externalContext,
            ITestOutputHelper testOutputHelper)
        {
            await Task.CompletedTask;
            var scl = Step.Serialize(SerializeOptions.Serialize);

            testOutputHelper.WriteLine(scl);

            var sfs = SettingsHelpers.CreateStepFactoryStore(
                new GraphSettings("", "", null),
                Assembly.GetAssembly(typeof(GraphConnection))!
            );

            var deserializedStep = SCLParsing.TryParseStep(scl);

            deserializedStep.ShouldBeSuccessful();

            var unfrozenStep = deserializedStep.Value.TryFreeze(SCLRunner.RootCallerMetadata, sfs);

            unfrozenStep.ShouldBeSuccessful();

            return unfrozenStep.Value;
        }

        /// <inheritdoc />
        public override void CheckUnitResult(Result<Unit, IError> result)
        {
            result.ShouldBeSuccessful();
        }

        /// <inheritdoc />
        public override void CheckOutputResult(Result<TOutput, IError> result)
        {
            result.ShouldBeSuccessful();
        }

        /// <inheritdoc />
        public override async Task<StateMonad> GetStateMonad(
            IExternalContext externalContext,
            ILogger logger)
        {
            var baseMonad = await base.GetStateMonad(externalContext, logger);

            return new StateMonad(
                baseMonad.Logger,
                baseMonad.StepFactoryStore,
                externalContext,
                baseMonad.SequenceMetadata
            );
        }
    }
}
