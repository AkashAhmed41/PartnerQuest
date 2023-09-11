using AutoMapper;
using BackendWebApi.Dataflow;
using BackendWebApi.Extensions;
using BackendWebApi.Interfaces;
using BackendWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BackendWebApi.SignalR;

[Authorize]
public class MessagesHub : Hub
{
    private readonly IMessageRepository _messageRepository;
    private readonly IMapper _mapper;
    private readonly IUserRepository _userRepository;
    public MessagesHub(IMessageRepository messageRepository, IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _messageRepository = messageRepository;
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUsername = httpContext.Request.Query["username"];
        var groupName = GetGroupName(Context.User.GetUsername(), otherUsername);

        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        await AddToMessageGroup(groupName);
        var messages = await _messageRepository.GetMessagesThread(Context.User.GetUsername(), otherUsername);

        await Clients.Group(groupName).SendAsync("ReceiveMessagesThread", messages);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await RemoveFromMessageGroup();
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(BuildMessageDataflow buildMessageDataflow)
    {
        var username = Context.User.GetUsername();
        if (username == buildMessageDataflow.RecipientUsername) 
            throw new HubException("You cannot send message to yourself!");

        var sender = await _userRepository.GetUserByUsernameAsync(username);
        var recipient = await _userRepository.GetUserByUsernameAsync(buildMessageDataflow.RecipientUsername);

        if (recipient == null) 
            throw new HubException("Recipient User Not Found!");

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            MessageContent = buildMessageDataflow.MessageContent
        };

        var groupName = GetGroupName(sender.UserName, recipient.UserName);
        var messageGroup = await _messageRepository.GetMessageGroup(groupName);

        if (messageGroup.Connections.Any(x => x.Username == recipient.UserName))
        {
            message.MessageRead = DateTime.UtcNow;
        }

        _messageRepository.AddMessage(message);
        if (await _messageRepository.SaveAllAsync())
        {
            await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDataflow>(message));
        }
    }

    private string GetGroupName (string caller, string otherUser)
    {
        var stringCompare = string.CompareOrdinal(caller, otherUser) < 0;
        return stringCompare ? $"{caller}-{otherUser}" : $"{otherUser}-{caller}";
    }

    private async Task<bool> AddToMessageGroup(string groupName)
    {
        var group = await _messageRepository.GetMessageGroup(groupName);
        var connection = new SignalRConnection(Context.ConnectionId, Context.User.GetUsername());

        if (group == null)
        {
            group = new SignalRGroup(groupName);
            _messageRepository.AddNewGroup(group);
        }

        group.Connections.Add(connection);
        return await _messageRepository.SaveAllAsync();
    }

    private async Task RemoveFromMessageGroup()
    {
        var connection = await _messageRepository.GetConnection(Context.ConnectionId);
        _messageRepository.RemoveConnection(connection);
        await _messageRepository.SaveAllAsync();
    }
}