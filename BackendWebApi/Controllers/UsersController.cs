using AutoMapper;
using BackendWebApi.Dataflow;
using BackendWebApi.Extensions;
using BackendWebApi.Helpers;
using BackendWebApi.Interfaces;
using BackendWebApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BackendWebApi.Controllers
{
    [Authorize]
    public class UsersController : BaseApiController
    {
        private readonly IMapper _mapper;
        private readonly IPhotoService _photoService;
        private readonly IUnitOfWork _unitOfWork;
        public UsersController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService)
        {
            _unitOfWork = unitOfWork;
            _photoService = photoService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<PaginatedList<MemberDataflow>>> GetAllUsers([FromQuery]UserParamsForPagination userParamsForPagination)
        {
            var gender = await _unitOfWork.UserRepository.GetUserGender(User.GetUsername());
            userParamsForPagination.CurrentUsername = User.GetUsername();

            if (string.IsNullOrEmpty(userParamsForPagination.Gender))
            {
                userParamsForPagination.Gender = gender == "male" ? "female" : "male";
            }

            var users = await _unitOfWork.UserRepository.GetMembersAsync(userParamsForPagination);
            Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));
            return Ok(users);
        }

        [HttpGet("{username}")]
        public async Task<ActionResult<MemberDataflow>> GetUser(string username)
        {
            return await _unitOfWork.UserRepository.GetMemberAsync(username);
        }

        [HttpPut]
        public async Task<ActionResult> EditMemberInfo(EditMemberInfoDataflow editMemberInfoDataflow)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            if (user ==  null) return NotFound("User not found!");

            _mapper.Map(editMemberInfoDataflow, user);
            if (await _unitOfWork.Complete()) return NoContent();

            return BadRequest("There was nothing to update!");
        }

        [HttpPost("add-photo")]
        public async Task<ActionResult<PhotoDataflow>> AddPhoto(IFormFile file)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
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

            if (await _unitOfWork.Complete()) 
            {
                return CreatedAtAction(nameof(GetUser), new {username = user.UserName}, _mapper.Map<PhotoDataflow>(photo));
            }

            return BadRequest("A problem occurred while adding your photo!");
        }

        [HttpPut("set-profile-photo/{photoId}")]
        public async Task<ActionResult> SettingProfilePhoto(int photoId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
            if (user == null) return NotFound();

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo == null) return NotFound();

            if (photo.IsProfilePhoto) return BadRequest("The selected photo is already your Profile Photo!");
            
            var currentProfilePhoto = user.Photos.FirstOrDefault(x => x.IsProfilePhoto);
            if (currentProfilePhoto != null) currentProfilePhoto.IsProfilePhoto = false;
            photo.IsProfilePhoto = true;

            if (await _unitOfWork.Complete()) return NoContent();
            return BadRequest("An unknown problem occurred while setting up your Profile Photo!");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var user = await _unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

            var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
            if (photo == null) return NotFound();

            if (photo.IsProfilePhoto) return BadRequest("You cannot delete your Profile Photo!");

            if (photo.PublicId != null)
            {
                var result = await _photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            user.Photos.Remove(photo);
            if (await _unitOfWork.Complete()) return Ok();

            return BadRequest("An unknown problem occurred while deleting the Photo!");
        }
    }
}