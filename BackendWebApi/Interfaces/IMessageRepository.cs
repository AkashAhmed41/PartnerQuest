using BackendWebApi.Dataflow;
using BackendWebApi.Helpers;
using BackendWebApi.Models;

namespace BackendWebApi.Interfaces;

public interface IMessageRepository
{
    void AddMessage(Message message);
    void DeleteMessage(Message message);
    Task<Message> GetMessage(int id);
    Task<PaginatedList<MessageDataflow>> GetUsersMessages();
    Task<IEnumerable<MessageDataflow>> GetMessagesThread(int currentUserId, int recipientId);
    Task<bool> SaveAllAsync();
}