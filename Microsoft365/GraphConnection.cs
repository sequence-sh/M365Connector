namespace Reductech.Sequence.Connectors.Microsoft365;

/// <summary>
/// A connection to Azure Graph
/// </summary>
public sealed class GraphConnection : IDisposable, IStateDisposable, ISCLObject
{
    private GraphConnection(GraphServiceClient graphServiceClient, AccessToken accessToken)
    {
        IsDisposed         = false;
        GraphServiceClient = graphServiceClient;
        AccessToken        = accessToken;
    }

    /// <summary>
    /// Create a connection from a pre-existing access token
    /// </summary>
    /// <returns></returns>
    public static Task<Result<GraphConnection, IErrorBuilder>>
        TryCreate(
            GraphSettings settings,
            string tokenString)
    {
        var accessToken = new AccessToken(tokenString, DateTime.Now + TimeSpan.FromDays(1));

        var d = DelegatedTokenCredential.Create((_, _) => accessToken);

        var client = new GraphServiceClient(d, settings.GraphUserScopes);

        return Task.FromResult<Result<GraphConnection, IErrorBuilder>>(
            new GraphConnection(client, accessToken)
        );
    }

    /// <summary>
    /// Create a graph connection by asking a user to go to a web page.
    /// </summary>
    public static async Task<Result<GraphConnection, IErrorBuilder>>
        TryCreate(
            GraphSettings settings,
            Func<DeviceCodeInfo, CancellationToken, Task> initGraph,
            CancellationToken cancellationToken)
    {
        var deviceCodeCredential = new DeviceCodeCredential(
            initGraph,
            settings.TenantId,
            settings.ClientId
        );

        var context  = new TokenRequestContext(settings.GraphUserScopes ?? Array.Empty<string>());
        var response = await deviceCodeCredential.GetTokenAsync(context, cancellationToken);

        var gsc = new GraphServiceClient(deviceCodeCredential, settings.GraphUserScopes);

        return new GraphConnection(gsc, response);
    }

    /// <summary>
    /// Returns true if the underlying connection has been disposed.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// The Graph Service Client
    /// </summary>
    public GraphServiceClient GraphServiceClient { get; }

    /// <summary>
    /// The Access Token
    /// </summary>
    public AccessToken AccessToken { get; }

    /// <inheritdoc />
    public string Serialize(SerializeOptions options)
    {
        return nameof(GraphConnection);
    }

    /// <inheritdoc />
    public TypeReference GetTypeReference()
    {
        return TypeReference.Unknown.Instance;
    }

    /// <inheritdoc />
    public IConstantFreezableStep ToConstantFreezableStep(TextLocation location)
    {
        throw new NotImplementedException("Cannot convert a graph connection to a Freezable step");
    }

    /// <inheritdoc />
    public Maybe<T> MaybeAs<T>() where T : ISCLObject
    {
        if (this is T t)
            return t;

        return Maybe<T>.None;
    }

    /// <inheritdoc />
    public object? ToCSharpObject()
    {
        return this;
    }

    /// <inheritdoc />
    public SchemaNode ToSchemaNode(string path, SchemaConversionOptions? schemaConversionOptions)
    {
        return FalseNode.Instance;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (!IsDisposed)
        {
            IsDisposed = true;
        }
    }

    /// <inheritdoc />
    public Task DisposeAsync(IStateMonad state)
    {
        if (!IsDisposed)
        {
            IsDisposed = true;
        }

        return Task.CompletedTask;
    }
}
