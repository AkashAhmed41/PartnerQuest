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
    private readonly IMapper _mapper;
    private readonly IHubContext<PresenceHub> _presenceHub;
    private readonly IUnitOfWork _unitOfWork;
    public MessagesHub(IUnitOfWork unitOfWork, IMapper mapper, IHubContext<PresenceHub> presenceHub)
    {
        _unitOfWork = unitOfWork;
        _presenceHub = presenceHub;
        _mapper = mapper;
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUsername = httpContext.Request.Query["username"];
        var groupName = GetGroupName(Context.User.GetUsername(), otherUsername);

        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        var messageGroup = await AddToMessageGroup(groupName);
        await Clients.Group(groupName).SendAsync("UpdatedGroup", messageGroup);

        var messages = await _unitOfWork.MessageRepository.GetMessagesThread(Context.User.GetUsername(), otherUsername);

        if (_unitOfWork.HasChanges()) await _unitOfWork.Complete();

        await Clients.Caller.SendAsync("ReceiveMessagesThread", messages);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var messageGroup = await RemoveFromMessageGroup();
        await Clients.Group(messageGroup.Name).SendAsync("UpdatedGroup");
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(BuildMessageDataflow buildMessageDataflow)
    {
        var username = Context.User.GetUsername();
        if (username == buildMessageDataflow.RecipientUsername) 
            throw new HubException("You cannot send message to yourself!");

        var sender = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
        var recipient = await _unitOfWork.UserRepository.GetUserByUsernameAsync(buildMessageDataflow.RecipientUsername);

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
        var messageGroup = await _unitOfWork.MessageRepository.GetMessageGroup(groupName);

        if (messageGroup.Connections.Any(x => x.Username == recipient.UserName))
        {
            message.MessageRead = DateTime.UtcNow;
        }
        else
        {
            var connections = await ActiveStatusTracker.GetUserConnections(recipient.UserName);
            if (connections != null)
            {
                await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", 
                    new { username = sender.UserName, nickname = sender.Nickname });
            }
        }

        _unitOfWork.MessageRepository.AddMessage(message);
        if (await _unitOfWork.Complete())
        {
            await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDataflow>(message));
        }
    }

    private string GetGroupName (string caller, string otherUser)
    {
        var stringCompare = string.CompareOrdinal(caller, otherUser) < 0;
        return stringCompare ? $"{caller}-{otherUser}" : $"{otherUser}-{caller}";
    }

    private async Task<SignalRGroup> AddToMessageGroup(string groupName)
    {
        var group = await _unitOfWork.MessageRepository.GetMessageGroup(groupName);
        var connection = new SignalRConnection(Context.ConnectionId, Context.User.GetUsername());

        if (group == null)
        {
            group = new SignalRGroup(groupName);
            _unitOfWork.MessageRepository.AddNewGroup(group);
        }

        group.Connections.Add(connection);
        if (await _unitOfWork.Complete()) return group;

        throw new HubException("Failed to add to message group!");
    }

    private async Task<SignalRGroup> RemoveFromMessageGroup()
    {
        var group = await _unitOfWork.MessageRepository.GetGroupFromConnection(Context.ConnectionId);
        var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
        _unitOfWork.MessageRepository.RemoveConnection(connection);

        if (await _unitOfWork.Complete()) return group;

        throw new HubException("Failed to remove from message group!");
    }
}