using BackendWebApi.Dataflow;
using BackendWebApi.Helpers;
using BackendWebApi.Models;

namespace BackendWebApi.Interfaces;

public interface IMessageRepository
{
    void AddMessage(Message message);
    void DeleteMessage(Message message);
    Task<Message> GetMessage(int id);
    Task<PaginatedList<MessageDataflow>> GetUsersMessages(MessagesPaginationParams messagesPaginationParams);
    Task<IEnumerable<MessageDataflow>> GetMessagesThread(string currentUsername, string recipientUsername);
    void AddNewGroup(SignalRGroup group);
    void RemoveConnection(SignalRConnection signalRConnection);
    Task<SignalRConnection> GetConnection(string connectionId);
    Task<SignalRGroup> GetMessageGroup(string groupName);
    Task<SignalRGroup> GetGroupFromConnection(string connectionId);
}