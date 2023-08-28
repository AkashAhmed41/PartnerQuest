using BackendWebApi.Dataflow;
using BackendWebApi.Interfaces;
using BackendWebApi.Models;
using BackendWebApi.Extensions;
using Microsoft.EntityFrameworkCore;

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

    public async Task<IEnumerable<FavouriteDataflow>> GetFavouriteUsersList(string predicate, int userId)
    {
        var users = _context.Users.OrderBy(u => u.UserName).AsQueryable();
        var favourites = _context.FavouriteUsersDb.AsQueryable();

        if (predicate == "loved")       //here loved means added favourite
        {
            favourites = favourites.Where(fav => fav.SourceUserId == userId);
            users = favourites.Select(fav => fav.FavouriteUser);        //here we're getting the favourite users of a sourceuser
        }

        if (predicate == "lovedBy")       //here lovedBy means added favourite by
        {
            favourites = favourites.Where(fav => fav.FavouriteUserId == userId);
            users = favourites.Select(fav => fav.SourceUser);        //here we're getting the users who have added a sourceuser as favourite
        }

        return await users.Select(user => new FavouriteDataflow
        {
            Id = user.Id,
            UserName = user.UserName,
            ProfilePhotoUrl = user.Photos.FirstOrDefault(photo => photo.IsProfilePhoto).PhotoUrl,
            Nickname = user.Nickname,
            Age = user.DateOfBirth.CalculateAge(),
            City = user.City
        }).ToListAsync();
    }

    public async Task<User> GetUserWithFavourite(int userId)
    {
        return await _context.Users.Include(user => user.AddedFavouriteUsers).FirstOrDefaultAsync(user => user.Id == userId);
    }
}