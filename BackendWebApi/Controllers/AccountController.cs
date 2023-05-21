using System.Security.Cryptography;
using System.Text;
using BackendWebApi.Database;
using BackendWebApi.Dataflow;
using BackendWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendWebApi.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        public AccountController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<User>> RegisterUser(RegisterDataflow registerDataflow)
        {
            if (await UserExists(registerDataflow.Username))
            {
                return BadRequest("Username is already taken!");
            }

            using var hmac = new HMACSHA512();

            var user = new User 
            {
                UserName = registerDataflow.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDataflow.Password)),
                PashwordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        [HttpPost("login")]
        public async Task<ActionResult<User>> UserLogin(LoginDataflow loginDataflow)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.UserName == loginDataflow.Username.ToLower());

            if (user == null)
            {
                return Unauthorized("Invalid Username!");
            }

            using var hmac = new HMACSHA512(user.PashwordSalt);

            var ComputedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDataflow.Password));

            for (int i=0; i<ComputedHash.Length; i++)
            {
                if (ComputedHash[i] != user.PasswordHash[i])
                {
                    return Unauthorized("Invalid Password!");
                }
            }

            return user;
        }

        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}