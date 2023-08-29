using BackendWebApi.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BackendWebApi.Extensions;
using BackendWebApi.Models;
using BackendWebApi.Dataflow;
using BackendWebApi.Helpers;

namespace BackendWebApi.Controllers;

public class FavouritesController : BaseApiController
{
    private readonly IFavouriteRepository _favouriteRepository;
    private readonly IUserRepository _userRepository;
    public FavouritesController(IUserRepository userRepository, IFavouriteRepository favouriteRepository)
    {
        _userRepository = userRepository;
        _favouriteRepository = favouriteRepository;
    }

    [HttpPost("{username}")]
    public async Task<ActionResult> AddFavourite(string username)
    {
        var sourceUserId = User.GetUserId();
        var addedFavouriteUser = await _userRepository.GetUserByUsernameAsync(username);
        var sourceUser = await _favouriteRepository.GetUserWithFavourite(sourceUserId);

        if (addedFavouriteUser == null) return NotFound();
        if (sourceUser.UserName == username) return BadRequest("You cannot add yourself as a favourite user!");

        var addFavouriteUser = await _favouriteRepository.GetFavouriteUser(sourceUserId, addedFavouriteUser.Id);
        if (addFavouriteUser != null) return BadRequest("You've already added this user as Favourite!");

        addFavouriteUser = new FavouriteUsers
        {
            SourceUserId = sourceUserId,
            FavouriteUserId = addedFavouriteUser.Id
        };
        sourceUser.AddedFavouriteUsers.Add(addFavouriteUser);

        if (await _userRepository.SaveAllAsync()) return Ok();

        return BadRequest("Failed to add as Favourite!");
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<FavouriteDataflow>>> GetFavouriteUserList([FromQuery]FavouritesPaginationParams favouritesPaginationParams)
    {
        favouritesPaginationParams.UserId = User.GetUserId();
        var users = await _favouriteRepository.GetFavouriteUsersList(favouritesPaginationParams);

        Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));

        return Ok(users);
    }
}