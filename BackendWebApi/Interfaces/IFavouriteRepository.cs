using BackendWebApi.Dataflow;
using BackendWebApi.Helpers;
using BackendWebApi.Models;

namespace BackendWebApi.Interfaces;

public interface IFavouriteRepository
{
    Task<FavouriteUsers> GetFavouriteUser(int sourceUserId, int favouriteUserId);
    Task<User> GetUserWithFavourite(int userId);
    Task<PaginatedList<FavouriteDataflow>> GetFavouriteUsersList(FavouritesPaginationParams favouritesPaginationParams);
}