namespace BackendWebApi.Interfaces;

public interface IUnitOfWork
{
    IUserRepository UserRepository { get; }
    IMessageRepository MessageRepository { get; }
    IFavouriteRepository FavouriteRepository { get; }
    Task<bool> Complete();
    bool HasChanges();
}