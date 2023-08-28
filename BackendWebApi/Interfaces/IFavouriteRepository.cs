using BackendWebApi.Dataflow;
using BackendWebApi.Models;

namespace BackendWebApi.Interfaces;

public interface IFavouriteRepository
{
    Task<FavouriteUsers> GetFavouriteUser(int sourceUserId, int favouriteUserId);
    Task<User> GetUserWithFavourite(int userId);
    Task<IEnumerable<FavouriteDataflow>> GetFavouriteUsersList(string predicate, int userId);
}