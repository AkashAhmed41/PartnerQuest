using System.Security.Claims;
using AutoMapper;
using BackendWebApi.Dataflow;
using BackendWebApi.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendWebApi.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDataflow>>> GetAllUsers()
        {
            return Ok(await _userRepository.GetMembersAsync());
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDataflow>> GetUser(string username)
        {
            return await _userRepository.GetMemberAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> EditMemberInfo(EditMemberInfoDataflow editMemberInfoDataflow)
        {
            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _userRepository.GetUserByUsernameAsync(username);
            if (user ==  null) return NotFound("User not found!");

            _mapper.Map(editMemberInfoDataflow, user);
            if (await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("There was nothing to update!");
        }
    }
}