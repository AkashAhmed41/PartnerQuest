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
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    public MessagesController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<ActionResult> BuildMessage(BuildMessageDataflow buildMessageDataflow)
    {
        var username = User.GetUsername();
        if (username == buildMessageDataflow.RecipientUsername) return BadRequest("You cannot send message to yourself!");

        var sender = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
        var recipient = await _unitOfWork.UserRepository.GetUserByUsernameAsync(buildMessageDataflow.RecipientUsername);

        if (recipient == null) return NotFound();

        var message = new Message
        {
            Sender = sender,
            Recipient = recipient,
            SenderUsername = sender.UserName,
            RecipientUsername = recipient.UserName,
            MessageContent = buildMessageDataflow.MessageContent
        };

        _unitOfWork.MessageRepository.AddMessage(message);
        if (await _unitOfWork.Complete()) return Ok(_mapper.Map<MessageDataflow>(message));

        return BadRequest("Failed to send message!");
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<MessageDataflow>>> GetUsersMessages([FromQuery]MessagesPaginationParams messagesPaginationParams)
    {
        messagesPaginationParams.Username = User.GetUsername();
        var messages = await _unitOfWork.MessageRepository.GetUsersMessages(messagesPaginationParams);
        
        Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages));
        return messages;
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMessage (int id)
    {
        var username = User.GetUsername();
        var message = await _unitOfWork.MessageRepository.GetMessage(id);

        if (message.SenderUsername != username && message.RecipientUsername != username) return Unauthorized();

        if (message.SenderUsername == username) message.SenderDeleted = true;
        if (message.RecipientUsername == username) message.RecipientDeleted = true;

        if (message.SenderDeleted && message.RecipientDeleted)
        {
            _unitOfWork.MessageRepository.DeleteMessage(message);
        }

        if (await _unitOfWork.Complete()) return Ok();
        return BadRequest("An error occurred while deleting the message!");
    }
}