using BackendWebApi.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace BackendWebApi.Controllers
{
    [ServiceFilter(typeof(TrackUserActivity))]
    [ApiController]
    [Route("api/[controller]")]
    public class BaseApiController : ControllerBase
    {
        
    }
}