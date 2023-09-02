using AutoMapper;
using AutoMapper.QueryableExtensions;
using BackendWebApi.Dataflow;
using BackendWebApi.Helpers;
using BackendWebApi.Interfaces;
using BackendWebApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendWebApi.Database;

public class MessageRepository : IMessageRepository
{
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    public MessageRepository(DataContext context, IMapper mapper)
    {
        _mapper = mapper;
        _context = context;
        
    }
    public void AddMessage(Message message)
    {
        _context.Messages.Add(message);
    }

    public void DeleteMessage(Message message)
    {
        _context.Messages.Remove(message);
    }

    public async Task<Message> GetMessage(int id)
    {
        return await _context.Messages.FindAsync(id);
    }

    public async Task<IEnumerable<MessageDataflow>> GetMessagesThread(string currentUsername, string recipientUsername)
    {
        var messages = await _context.Messages.Include(msg => msg.Sender).ThenInclude(u => u.Photos).Where(msg => 
            msg.SenderUsername == currentUsername && msg.SenderDeleted == false && msg.RecipientUsername == recipientUsername ||
            msg.RecipientUsername == currentUsername && msg.RecipientDeleted == false && msg.SenderUsername == recipientUsername
        ).OrderBy(msg => msg.MessageSent).ToListAsync();

        var unreadMessages = messages.Where(msg => msg.MessageRead == null && msg.RecipientUsername == currentUsername).ToList();
        if (unreadMessages.Any())
        {
            foreach (var message in unreadMessages)
            {
                message.MessageRead = DateTime.UtcNow;
            }
            await _context.SaveChangesAsync();
        }
        
        return _mapper.Map<IEnumerable<MessageDataflow>>(messages);
    }

    public async Task<PaginatedList<MessageDataflow>> GetUsersMessages(MessagesPaginationParams messagesPaginationParams)
    {
        var query = _context.Messages.OrderByDescending(msg => msg.MessageSent).AsQueryable();
        query = messagesPaginationParams.Container switch
        {
            "Inbox" => query.Where(msg => msg.RecipientUsername == messagesPaginationParams.Username && msg.RecipientDeleted == false),
            "Outbox" => query.Where(msg => msg.SenderUsername == messagesPaginationParams.Username && msg.SenderDeleted == false),
            _ => query.Where(msg => msg.RecipientUsername == messagesPaginationParams.Username && msg.RecipientDeleted == false && msg.MessageRead == null)
        };

        var messages = query.ProjectTo<MessageDataflow>(_mapper.ConfigurationProvider);
        return await PaginatedList<MessageDataflow>.CreatePageAsync(messages, messagesPaginationParams.PageNumber, messagesPaginationParams.PageSize);
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}