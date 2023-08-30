using BackendWebApi.Dataflow;
using BackendWebApi.Helpers;
using BackendWebApi.Interfaces;
using BackendWebApi.Models;

namespace BackendWebApi.Database;

public class MessageRepository : IMessageRepository
{
    private readonly DataContext _context;
    public MessageRepository(DataContext context)
    {
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

    public Task<IEnumerable<MessageDataflow>> GetMessagesThread(int currentUserId, int recipientId)
    {
        throw new NotImplementedException();
    }

    public Task<PaginatedList<MessageDataflow>> GetUsersMessages()
    {
        throw new NotImplementedException();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync() > 0;
    }
}