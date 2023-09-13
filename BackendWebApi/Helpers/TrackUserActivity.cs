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
        var unitOfWork = resultContext.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
        var user = await unitOfWork.UserRepository.GetUserByIdAsync(userId);
        user.LastActive = DateTime.UtcNow;
        await unitOfWork.Complete();
    }
}