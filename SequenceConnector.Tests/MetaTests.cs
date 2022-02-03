using System.Reflection;
using Reductech.Sequence.Core.TestHarness;

namespace Reductech.Templates.SequenceConnector.Tests;

/// <summary>
/// Makes sure all steps have a test class
/// </summary>
public class MetaTests : MetaTestsBase
{
    /// <inheritdoc />
    public override Assembly StepAssembly => typeof(ConvertJsonToEntity).Assembly;

    /// <inheritdoc />
    public override Assembly TestAssembly => typeof(MetaTests).Assembly;
}
