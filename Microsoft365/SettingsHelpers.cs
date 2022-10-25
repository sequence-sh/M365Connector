using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Logging;
using Reductech.Sequence.ConnectorManagement.Base;
using Reductech.Sequence.Core.Abstractions;

namespace Reductech.Sequence.Connectors.Microsoft365;

/// <summary>
/// Methods to help with Microsoft Graph settings
/// </summary>
public static class SettingsHelpers
{
    /// <summary>
    /// The name of the Graph Variable in the SCL state
    /// </summary>
    internal static readonly VariableName GraphVariableName =
        new("ReductechGraphConnection");

    private const string M365Key = "Reductech.Sequence.Connectors.Microsoft365";

    /// <summary>
    /// Try to get a list of Settings from the global settings Entity
    /// </summary>
    public static Result<GraphSettings, IErrorBuilder> TryGetGraphSettings(Entity settings)
    {
        var m365Connector = settings.TryGetValue(
            new EntityNestedKey(
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
    /// Forget existing connection to Microsoft 365
    /// </summary>
    public static async Task<Result<Unit, IError>> ForgetGraphConnection(
        this IStateMonad stateMonad,
        IStep callingStep)
    {
        await stateMonad.RemoveVariableAsync(GraphVariableName, false, callingStep);
        return Unit.Default;
    }

    /// <summary>
    /// This creates a new connection to Microsoft 365.
    /// If there is a preexisting connection, it is discarded.
    /// </summary>
    public static async Task<Result<GraphConnection, IError>> CreateGraphConnection(
        this IStateMonad stateMonad,
        IStep callingStep,
        string tokenString,
        CancellationToken cancellationToken)
    {
        var graphSettings = TryGetGraphSettings(stateMonad.Settings);

        if (graphSettings.IsFailure)
            return graphSettings.ConvertFailure<GraphConnection>()
                .MapError(x => x.WithLocation(callingStep));

        var newConnection = await GraphConnection.TryCreate(graphSettings.Value, tokenString);

        if (newConnection.IsFailure)
            return newConnection.ConvertFailure<GraphConnection>()
                .MapError(x => x.WithLocation(callingStep));

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
    /// The default InitGraph function
    /// </summary>
    /// <returns></returns>
    public static Task DefaultInitGraph(
        DeviceCodeInfo cdi,
        IStateMonad stateMonad,
        IStep callingStep,
        CancellationToken _)
    {
        stateMonad.Log(LogLevel.Information, cdi.Message, callingStep);
        return Task.FromResult(0);
    }

    /// <summary>
    /// Gets or creates a connection to Microsoft 365.
    /// If this is the first time getting a connection, the user is required to click a link and login.
    /// </summary>
    public static async Task<Result<GraphConnection, IError>> GetOrCreateGraphConnection(
        this IStateMonad stateMonad,
        IStep callingStep,
        Func<DeviceCodeInfo, IStateMonad, IStep, CancellationToken, Task> initGraph,
        CancellationToken cancellationToken)
    {
        var currentConnection = stateMonad.GetVariable<GraphConnection>(GraphVariableName);

        if (currentConnection.IsSuccess)
            return currentConnection.MapError(x => x.WithLocation(callingStep));

        var graphSettings = TryGetGraphSettings(stateMonad.Settings);

        if (graphSettings.IsFailure)
            return graphSettings.ConvertFailure<GraphConnection>()
                .MapError(x => x.WithLocation(callingStep));

        var newConnection = await GraphConnection.TryCreate(
            graphSettings.Value,
            (info, token) => initGraph(info, stateMonad, callingStep, token),
            cancellationToken
        );

        if (newConnection.IsFailure)
            return newConnection.MapError(x => x.WithLocation(callingStep));

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
            .ToDictionary(
                k => k.Key.Inner,
                v => v.Value.ToCSharpObject(),
                StringComparer.OrdinalIgnoreCase
            )!;

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
