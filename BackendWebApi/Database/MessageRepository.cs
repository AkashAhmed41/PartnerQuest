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

    public void AddNewGroup(SignalRGroup group)
    {
        _context.MessagesGroups.Add(group);
    }

    public void DeleteMessage(Message message)
    {
        _context.Messages.Remove(message);
    }

    public async Task<SignalRConnection> GetConnection(string connectionId)
    {
        return await _context.Connections.FindAsync(connectionId);
    }

    public async Task<SignalRGroup> GetGroupFromConnection(string connectionId)
    {
        return await _context.MessagesGroups.Include(x => x.Connections)
            .Where(x => x.Connections.Any(x => x.ConnectionId == connectionId))
            .FirstOrDefaultAsync();
    }

    public async Task<Message> GetMessage(int id)
    {
        return await _context.Messages.FindAsync(id);
    }

    public async Task<SignalRGroup> GetMessageGroup(string groupName)
    {
        return await _context.MessagesGroups.Include(x => x.Connections).FirstOrDefaultAsync(x => x.Name == groupName);
    }

    public async Task<IEnumerable<MessageDataflow>> GetMessagesThread(string currentUsername, string recipientUsername)
    {
        var query = _context.Messages.Where(msg => 
            msg.SenderUsername == currentUsername && msg.SenderDeleted == false && msg.RecipientUsername == recipientUsername ||
            msg.RecipientUsername == currentUsername && msg.RecipientDeleted == false && msg.SenderUsername == recipientUsername
        ).OrderBy(msg => msg.MessageSent).AsQueryable();

        var unreadMessages = query.Where(msg => msg.MessageRead == null && msg.RecipientUsername == currentUsername).ToList();
        if (unreadMessages.Any())
        {
            foreach (var message in unreadMessages)
            {
                message.MessageRead = DateTime.UtcNow;
            }
        }
        
        return await query.ProjectTo<MessageDataflow>(_mapper.ConfigurationProvider).ToListAsync();
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

    public void RemoveConnection(SignalRConnection signalRConnection)
    {
        _context.Connections.Remove(signalRConnection);
    }

}