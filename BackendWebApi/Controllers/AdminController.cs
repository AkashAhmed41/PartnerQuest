using BackendWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendWebApi.Controllers;

public class AdminController : BaseApiController
{
    private readonly UserManager<User> _userManager;
    public AdminController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult> GetUsersWithRoles()
    {
        var users = await _userManager.Users.OrderBy(u => u.UserName)
                            .Select(u => new 
                            {
                                u.Id,
                                Username = u.UserName,
                                Roles = u.UserRoles.Select(r => r.Role.Name).ToList()
                            })
                            .ToListAsync();
        
        return Ok(users);
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("edit-user-roles/{username}")]
    public async Task<ActionResult> EditUserRoles(string username, [FromQuery]string roles)
    {
        if (string.IsNullOrEmpty(roles)) return BadRequest("You must select a Role. An user cannot be Roleless!");

        var selectedRoles = roles.Split(",").ToArray();
        var user = await _userManager.FindByNameAsync(username);
        if (user == null) return NotFound();

        var userRoles = await _userManager.GetRolesAsync(user);
        var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

        if (!result.Succeeded) return BadRequest("Failed to update Roles for the Selected User!");

        result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
        if (!result.Succeeded) return BadRequest("Failed to update Roles for the Selected User!");

        return Ok(await _userManager.GetRolesAsync(user));
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpGet("photos-to-moderate")]
    public ActionResult GetPhotosToModerate()
    {
        return Ok("Admins or Moderators are allowed!");
    }
}