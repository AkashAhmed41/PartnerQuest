using BackendWebApi.Extensions;
using BackendWebApi.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BackendWebApi.Helpers;

public class TrackUserActivity : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next();

        if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

        var userId = resultContext.HttpContext.User.GetUserId();
        var userRepository = resultContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
        var user = await userRepository.GetUserByIdAsync(userId);
        user.LastActive = DateTime.UtcNow;
        await userRepository.SaveAllAsync();
    }
}