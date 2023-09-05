using AutoMapper;
using BackendWebApi.Dataflow;
using BackendWebApi.Interfaces;
using BackendWebApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendWebApi.Controllers
{
    public class AccountController : BaseApiController
    {
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        public AccountController(UserManager<User> userManager, ITokenService tokenService, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDataflow>> RegisterUser(RegisterDataflow registerDataflow)
        {
            if (await UserExists(registerDataflow.Username))
            {
                return BadRequest("Username is already taken!");
            }

            var user = _mapper.Map<User>(registerDataflow);

            user.UserName = registerDataflow.Username.ToLower();

            var result = await _userManager.CreateAsync(user, registerDataflow.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            var roleResult = await _userManager.AddToRoleAsync(user, "Member");
            if (!roleResult.Succeeded) return BadRequest(roleResult.Errors);

            return new UserDataflow
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                Nickname = user.Nickname,
                Gender = user.Gender
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDataflow>> UserLogin(LoginDataflow loginDataflow)
        {
            var user = await _userManager.Users.Include(user => user.Photos).SingleOrDefaultAsync(x => x.UserName == loginDataflow.Username.ToLower());

            if (user == null)
            {
                return Unauthorized("Invalid Username!");
            }

            var result = await _userManager.CheckPasswordAsync(user, loginDataflow.Password);
            if (!result) return Unauthorized("Invalid Password!");

            return new UserDataflow
            {
                Username = user.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsProfilePhoto)?.PhotoUrl,
                Nickname = user.Nickname,
                Gender = user.Gender
            };
        }

        private async Task<bool> UserExists(string username)
        {
            return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
        }
    }
}