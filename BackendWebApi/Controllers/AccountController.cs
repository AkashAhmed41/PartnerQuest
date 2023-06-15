using System.Security.Cryptography;
using System.Text;
using BackendWebApi.Database;
using BackendWebApi.Dataflow;
using BackendWebApi.Interfaces;
using BackendWebApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendWebApi.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly DataContext _context;
        private readonly ITokenService _tokenService;
        public AccountController(DataContext context, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDataflow>> RegisterUser(RegisterDataflow registerDataflow)
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
                PasswordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDataflow
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user)
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDataflow>> UserLogin(LoginDataflow loginDataflow)
        {
            var user = await _context.Users.Include(user => user.Photos).SingleOrDefaultAsync(x => x.UserName == loginDataflow.Username.ToLower());

            if (user == null)
            {
                return Unauthorized("Invalid Username!");
            }

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var ComputedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDataflow.Password));

            for (int i=0; i<ComputedHash.Length; i++)
            {
                if (ComputedHash[i] != user.PasswordHash[i])
                {
                    return Unauthorized("Invalid Password!");
                }
            }

            return new UserDataflow
            {
                Username = user.UserName,
                Token = _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsProfilePhoto)?.PhotoUrl
            };
        }

        private async Task<bool> UserExists(string username)
        {
            return await _context.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}