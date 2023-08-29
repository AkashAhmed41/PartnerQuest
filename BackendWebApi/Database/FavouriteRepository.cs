using BackendWebApi.Dataflow;
using BackendWebApi.Interfaces;
using BackendWebApi.Models;
using BackendWebApi.Extensions;
using Microsoft.EntityFrameworkCore;
using BackendWebApi.Helpers;

namespace BackendWebApi.Database;

public class FavouriteRepository : IFavouriteRepository
{
    private readonly DataContext _context;
    public FavouriteRepository(DataContext context)
    {
        _context = context;
        
    }
    public async Task<FavouriteUsers> GetFavouriteUser(int sourceUserId, int favouriteUserId)
    {
        return await _context.FavouriteUsersDb.FindAsync(sourceUserId, favouriteUserId);
    }

    public async Task<PaginatedList<FavouriteDataflow>> GetFavouriteUsersList(FavouritesPaginationParams favouritesPaginationParams)
    {
        var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
        var favourites = _context.FavouriteUsersDb.AsQueryable();

        if (favouritesPaginationParams.Predicate == "loved")       //here loved means added favourite
        {
            favourites = favourites.Where(fav => fav.SourceUserId == favouritesPaginationParams.UserId);
            users = favourites.Select(fav => fav.FavouriteUser);        //here we're getting the favourite users of a sourceuser
        }

        if (favouritesPaginationParams.Predicate == "lovedBy")       //here lovedBy means added favourite by
        {
            favourites = favourites.Where(fav => fav.FavouriteUserId == favouritesPaginationParams.UserId);
            users = favourites.Select(fav => fav.SourceUser);        //here we're getting the users who have added a sourceuser as favourite
        }

        var favouriteUsers = users.Select(user => new FavouriteDataflow
        {
            Id = user.Id,
            UserName = user.UserName,
            ProfilePhotoUrl = user.Photos.FirstOrDefault(photo => photo.IsProfilePhoto).PhotoUrl,
            Nickname = user.Nickname,
            Age = user.DateOfBirth.CalculateAge(),
            City = user.City
        });

        return await PaginatedList<FavouriteDataflow>.CreatePageAsync(favouriteUsers, favouritesPaginationParams.PageNumber, favouritesPaginationParams.PageSize);
    }

    public async Task<User> GetUserWithFavourite(int userId)
    {
        return await _context.Users.Include(user => user.AddedFavouriteUsers).FirstOrDefaultAsync(user => user.Id == userId);
    }
}