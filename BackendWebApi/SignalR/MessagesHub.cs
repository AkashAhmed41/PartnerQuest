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
        var messages = await _messageRepository.GetMessagesThread(Context.User.GetUsername(), otherUsername);

        await Clients.Group(groupName).SendAsync("ReceiveMessagesThread", messages);
    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        return base.OnDisconnectedAsync(exception);
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

        _messageRepository.AddMessage(message);
        if (await _messageRepository.SaveAllAsync())
        {
            var groupName = GetGroupName(sender.UserName, recipient.UserName);
            await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDataflow>(message));
        }
    }

    private string GetGroupName (string caller, string otherUser)
    {
        var stringCompare = string.CompareOrdinal(caller, otherUser) < 0;
        return stringCompare ? $"{caller}-{otherUser}" : $"{otherUser}-{caller}";
    }
}