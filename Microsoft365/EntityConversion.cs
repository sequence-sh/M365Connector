//using Microsoft.Graph.Models;

namespace Sequence.Connectors.Microsoft365;

/// <summary>
/// Methods to convert Microsoft Graph Objects to SCL Entities
/// </summary>
public static class EntityConversion
{
    /// <summary>
    /// Convert this user to an entity
    /// </summary>
    public static Entity ToEntity(this User user)
    {
        return Entity.Create((nameof(User.DisplayName), user.DisplayName));
    }

    /// <summary>
    /// Convert this message to an entity
    /// </summary>
    public static Entity ToEntity(this Message message)
    {
        return Entity.Create(
            (nameof(Message.From), message.From.EmailAddress.Name),
            (nameof(Message.IsRead), message.IsRead),
            (nameof(Message.ReceivedDateTime), message.ReceivedDateTime?.DateTime),
            (nameof(Message.Subject), message.Subject),
            (nameof(Message.Body), message.Body.Content)
        );
    }

    /// <summary>
    /// Convert this identity to an entity
    /// </summary>
    public static Entity ToEntity(this Identity user)
    {
        return Entity.Create((nameof(User.DisplayName), user.DisplayName));
    }

    /// <summary>
    /// Convert this channel to an entity
    /// </summary>
    public static Entity ToEntity(this Channel channel)
    {
        return Entity.Create(
            (nameof(Channel.Id), channel.Id),
            (nameof(Channel.Description), channel.Description),
            (nameof(Channel.DisplayName), channel.DisplayName)
        );
    }

    /// <summary>
    /// Convert this team to an entity
    /// </summary>
    public static Entity ToEntity(this Team team)
    {
        //m.DisplayName, m.Description, m.Members
        return Entity.Create(
            (nameof(Team.Id), team.Id),
            (nameof(Team.DisplayName), team.DisplayName),
            (nameof(Team.Description), team.Description),
            (nameof(Team.Members), team.Members?.Select(member => member.ToEntity()))
        );
    }

    /// <summary>
    /// Convert this chat to an entity
    /// </summary>
    public static Entity ToEntity(this Chat chat)
    {
        return Entity.Create(
            (nameof(Chat.Topic), chat.Topic),
            (nameof(Chat.Members),
             chat.Members?.Select(mem => mem.ToEntity()) ?? ArraySegment<Entity>.Empty),
            (nameof(Chat.Messages),
             chat.Messages?.Select(message => message.ToEntity()) ?? ArraySegment<Entity>.Empty)
        );
    }

    /// <summary>
    /// Convert this conversation member to an entity
    /// </summary>
    public static Entity ToEntity(this ConversationMember member)
    {
        return Entity.Create(
            (nameof(ConversationMember.DisplayName), member.DisplayName),
            (nameof(ConversationMember.Id), member.Id)
        );
    }

    /// <summary>
    /// Convert this chat message to an entity
    /// </summary>
    public static Entity ToEntity(this ChatMessage message)
    {
        //TODO get replies

        return Entity.Create(
            (nameof(ChatMessage.Subject), message.Subject),
            (nameof(ChatMessage.Summary), message.Summary),
            (nameof(ChatMessage.Body), message.Body?.Content),
            (nameof(ChatMessage.From), message.From?.User.ToEntity())
        );
    }
}
