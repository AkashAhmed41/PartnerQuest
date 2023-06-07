using AutoMapper;
using BackendWebApi.Dataflow;
using BackendWebApi.Extensions;
using BackendWebApi.Interfaces;
using BackendWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendWebApi.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
        {
            _photoService = photoService;
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
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            if (user ==  null) return NotFound("User not found!");

            _mapper.Map(editMemberInfoDataflow, user);
            if (await _userRepository.SaveAllAsync()) return NoContent();

            return BadRequest("There was nothing to update!");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDataflow>> AddPhoto(IFormFile file)
        {
            var user = await _userRepository.GetUserByUsernameAsync(User.GetUsername());
            if (user == null) return NotFound();

            var result = await _photoService.AddPhotoAsync(file);
            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                PhotoUrl = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId
            };
            if (user.Photos.Count == 0) photo.IsProfilePhoto = true;
            user.Photos.Add(photo);

            if (await _userRepository.SaveAllAsync()) 
            {
                return CreatedAtAction(nameof(GetUser), new {username = user.UserName}, _mapper.Map<PhotoDataflow>(photo));
            }

            return BadRequest("A problem occurred while adding your photo!");
        }
    }
}