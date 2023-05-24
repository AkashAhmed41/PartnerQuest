using BackendWebApi.Database;
using BackendWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendWebApi.Controllers
{
    public class ErrorController : BaseApiController
    {
        private readonly DataContext _context;
        public ErrorController(DataContext context)
        {
            _context = context;
        }

        [Authorize]
        [HttpGet("auth")]
        public ActionResult<string> GetSecret() 
        {
            return "secret string";
        }

        [HttpGet("not-found")]
        public ActionResult<User> GetNotFound() 
        {
            var user = _context.Users.Find(-1);

            if (user == null) return NotFound(); 

            return user;
        }

        [HttpGet("server-error")]
        public ActionResult<string> GetSeverError() 
        {
            var user = _context.Users.Find(-1);
            var toReturn = user.ToString();
            return toReturn;
        }

        [HttpGet("bad-request")]
        public ActionResult<string> GetBadRequest() 
        {
            return BadRequest("This was a Bad Request!");
        }
    }
}