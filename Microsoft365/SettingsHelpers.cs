using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Azure.Identity;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Reductech.Sequence.ConnectorManagement.Base;
using Reductech.Sequence.Core;
using Reductech.Sequence.Core.Abstractions;
using Reductech.Sequence.Core.Internal;
using Reductech.Sequence.Core.Internal.Errors;
using Entity = Reductech.Sequence.Core.Entity;

namespace Reductech.Sequence.Connectors.Microsoft365;

public static class SettingsHelpers
{
    /// <summary>
    /// The name of the Graph Variable in the SCL state
    /// </summary>
    internal static readonly VariableName GraphVariableName =
        new("ReductechGraphConnection");

    public const string M365Key = "Reductech.Sequence.Connectors.Microsoft365";

    /// <summary>
    /// Try to get a list of NuixSettings from the global settings Entity
    /// </summary>
    public static Result<GraphSettings, IErrorBuilder> TryGetGraphSettings(Entity settings)
    {
        var m365Connector = settings.TryGetValue(
            new EntityPropertyKey(
                StateMonad.ConnectorsKey,
                M365Key,
                nameof(ConnectorData.ConnectorSettings.Settings)
            )
        );

        if (m365Connector.HasNoValue
         || m365Connector.GetValueOrThrow() is not Entity ent)
            return ErrorCode.MissingStepSettings.ToErrorBuilder(M365Key);

        var settingsObj =
            EntityConversionHelpers.TryCreateFromEntity<GraphSettings>(ent);

        return settingsObj;
    }

    /// <summary>
    /// Gets or creates a connection to Microsoft 365.
    /// </summary>
    public static async Task<Result<GraphConnection, IError>> GetOrCreateGraphConnection(
        this IStateMonad stateMonad,
        //Func<DeviceCodeInfo, CancellationToken, Task> initGraph,
        IStep callingStep,
        CancellationToken cancellationToken)
    {
        var currentConnection = stateMonad.GetVariable<GraphConnection>(GraphVariableName);

        if (currentConnection.IsSuccess)
            return currentConnection.MapError(x => x.WithLocation(callingStep));

        var graphSettings = TryGetGraphSettings(stateMonad.Settings);

        if (graphSettings.IsFailure)
            return graphSettings.ConvertFailure<GraphConnection>()
                .MapError(x => x.WithLocation(callingStep));

        Task InitGraph(DeviceCodeInfo cdi, CancellationToken ct)
        {
            stateMonad.Log(LogLevel.Information, cdi.Message, callingStep);
            return Task.FromResult(0);
        }

        var newConnection = await GraphConnection.TryCreate(
            graphSettings.Value,
            InitGraph,
            cancellationToken
        );

        if (newConnection.IsFailure)
            return newConnection.MapError(x => x.WithLocation(callingStep));

        //newConnection.Value.GraphServiceClient.

        var result = await stateMonad.SetVariableAsync(
            GraphVariableName,
            newConnection.Value,
            true,
            callingStep,
            cancellationToken
        );

        if (result.IsFailure)
            return result.ConvertFailure<GraphConnection>();

        return newConnection.Value;
    }

    /// <summary>
    /// Create Graph Settings
    /// </summary>
    public static StepFactoryStore CreateStepFactoryStore(
        GraphSettings graphSettings,
        params Assembly[] additionalAssemblies)
    {
        var assembly = Assembly.GetAssembly(typeof(GraphSettings));

        var ns = ConnectorSettings.DefaultForAssembly(assembly!);

        ns.Settings = graphSettings.ConvertToEntity()
            .Dictionary.ToDictionary(k => k.Key, v => v.Value.Value.ToCSharpObject()!);

        var core = Assembly.GetAssembly(typeof(IStep));

        var cd = new List<ConnectorData>
        {
            new(ConnectorSettings.DefaultForAssembly(core!), core), new(ns, assembly)
        };

        cd.AddRange(
            additionalAssemblies
                .Where(x => x != assembly)
                .Select(a => new ConnectorData(ConnectorSettings.DefaultForAssembly(a), a))
        );

        var sfs = StepFactoryStore.TryCreate(ExternalContext.Default, cd.ToArray())
            .Value; //TODO inject the external context

        return sfs;
    }
}
