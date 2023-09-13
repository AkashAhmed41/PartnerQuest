using BackendWebApi.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BackendWebApi.Extensions;
using BackendWebApi.Models;
using BackendWebApi.Dataflow;
using BackendWebApi.Helpers;

namespace BackendWebApi.Controllers;

public class FavouritesController : BaseApiController
{
    private readonly IUnitOfWork _unitOfWork;
    public FavouritesController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
        
    }

    [HttpPost("{username}")]
    public async Task<ActionResult> AddFavourite(string username)
    {
        var sourceUserId = User.GetUserId();
        var addedFavouriteUser = await _unitOfWork.UserRepository.GetUserByUsernameAsync(username);
        var sourceUser = await _unitOfWork.FavouriteRepository.GetUserWithFavourite(sourceUserId);

        if (addedFavouriteUser == null) return NotFound();
        if (sourceUser.UserName == username) return BadRequest("You cannot add yourself as a favourite user!");

        var addFavouriteUser = await _unitOfWork.FavouriteRepository.GetFavouriteUser(sourceUserId, addedFavouriteUser.Id);
        if (addFavouriteUser != null) return BadRequest("You've already added this user as Favourite!");

        addFavouriteUser = new FavouriteUsers
        {
            SourceUserId = sourceUserId,
            FavouriteUserId = addedFavouriteUser.Id
        };
        sourceUser.AddedFavouriteUsers.Add(addFavouriteUser);

        if (await _unitOfWork.Complete()) return Ok();

        return BadRequest("Failed to add as Favourite!");
    }

    [HttpGet]
    public async Task<ActionResult<PaginatedList<FavouriteDataflow>>> GetFavouriteUserList([FromQuery]FavouritesPaginationParams favouritesPaginationParams)
    {
        favouritesPaginationParams.UserId = User.GetUserId();
        var users = await _unitOfWork.FavouriteRepository.GetFavouriteUsersList(favouritesPaginationParams);

        Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));

        return Ok(users);
    }
}