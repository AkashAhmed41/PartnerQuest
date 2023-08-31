using AutoMapper;
using BackendWebApi.Dataflow;
using BackendWebApi.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BackendWebApi.Extensions;
using BackendWebApi.Models;
using BackendWebApi.Helpers;

namespace BackendWebApi.Controllers;

public class MessagesController : BaseApiController
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    public MessagesController(IUserRepository userRepository, IMessageRepository messageRepository, IMapper mapper)
    {
        _mapper = mapper;
        _userRepository = userRepository;
        _messageRepository = messageRepository;
    }

    [HttpPost]
    public async Task<ActionResult> BuildMessage(BuildMessageDataflow buildMessageDataflow)
    {
        var username = User.GetUsername();
        if (username == buildMessageDataflow.RecipientUsername) return BadRequest("You cannot send message to yourself!");

        var sender = await _userRepository.GetUserByUsernameAsync(username);
        var recipient = await _userRepository.GetUserByUsernameAsync(buildMessageDataflow.RecipientUsername);

        if (recipient == null) return NotFound();

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            MessageContent = buildMessageDataflow.MessageContent
        };

        _messageRepository.AddMessage(message);
        if (await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDataflow>(message));

        return BadRequest("Failed to send message!");
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<MessageDataflow>>> GetUsersMessages([FromQuery]MessagesPaginationParams messagesPaginationParams)
    {
        messagesPaginationParams.Username = User.GetUsername();
        var messages = await _messageRepository.GetUsersMessages(messagesPaginationParams);
        
        Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages));
        return messages;
    }

    [HttpGet("thread/{username}")]
    public async Task<ActionResult<IEnumerable<MessageDataflow>>> GetMessagesThread(string username)
    {
        var currentUsername = User.GetUsername();
        return Ok(await _messageRepository.GetMessagesThread(currentUsername, username));
    }
}