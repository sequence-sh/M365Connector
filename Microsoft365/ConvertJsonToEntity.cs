using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using Reductech.Sequence.Core;
using Reductech.Sequence.Core.Attributes;
using Reductech.Sequence.Core.Entities;
using Reductech.Sequence.Core.Internal;
using Reductech.Sequence.Core.Internal.Errors;
using Entity = Reductech.Sequence.Core.Entity;

namespace Reductech.Sequence.Connectors.Microsoft365;

/// <summary>
/// Convert an entity from a JSON stream. Clone of StructuredData.FromJSON.
/// </summary>
public sealed class ConvertJsonToEntity : CompoundStep<Entity>
{
    /// <inheritdoc />
    protected override async Task<Result<Entity, IError>> Run(
        IStateMonad stateMonad,
        CancellationToken cancellationToken)
    {
        var text = await Stream.Run(stateMonad, cancellationToken).Map(x => x.GetStringAsync());

        if (text.IsFailure)
            return text.ConvertFailure<Entity>();

        Entity? entity;

        try
        {
            entity = JsonSerializer.Deserialize<Entity>(
                text.Value,
                new JsonSerializerOptions()
                {
                    Converters =
                    {
                        new JsonStringEnumConverter(), VersionJsonConverter.Instance
                    }
                }
            );
        }
        catch (Exception e)
        {
            stateMonad.Log(LogLevel.Error, e.Message, this);
            entity = null;
        }

        if (entity is null)
            return Result.Failure<Entity, IError>(
                ErrorCode.CouldNotParse.ToErrorBuilder(text.Value, "JSON").WithLocation(this)
            );

        return entity;
    }

    /// <summary>
    /// Stream containing the Json data.
    /// </summary>
    [StepProperty(1)]
    [Required]
    public IStep<StringStream> Stream { get; set; } = null!;

    /// <inheritdoc />
    public override IStepFactory StepFactory { get; } =
        new SimpleStepFactory<ConvertJsonToEntity, Entity>();
}
