using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using datingApp.api.Data;
using datingApp.api.DTOs;
using datingApp.api.Helpers;
using datingApp.api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace datingApp.api.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotoController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudaniryConfig;
        private Cloudinary _cloudinary;

        public PhotoController(IDatingRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudaniryConfig)
        {
            _cloudaniryConfig = cloudaniryConfig;
            _mapper = mapper;
            _repo = repo;

            Account acc = new Account(
                _cloudaniryConfig.Value.CloudName,
                _cloudaniryConfig.Value.ApiKey,     
                _cloudaniryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);

        }

        [HttpGet("{id}", Name="GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo = await _repo.GetPhoto(id);
            var photo = _mapper.Map<PhotoToReturnDTO>(photoFromRepo);
            return Ok(photo); 
        }

        [HttpPost]
        public async Task<IActionResult> addPhotoForUser(int userId, [FromForm]PhotoToStoreDTO photoToStoreDTO)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await _repo.GetUser(userId);
            var file = photoToStoreDTO.File;                                
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using( var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };
                    uploadResult = _cloudinary.Upload(uploadParams);
 
                }
            }

            photoToStoreDTO.Url = uploadResult.Url.ToString();
            photoToStoreDTO.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photoToStoreDTO);

            if (!userFromRepo.Photos.Any(u => u.IsMain))                                    
                photo.IsMain = true; 
            
            userFromRepo.Photos.Add(photo);
            

            if (await _repo.SaveAll())
            {
                var phootToReturn = _mapper.Map<PhotoToReturnDTO>(photo);
                return CreatedAtRoute("GetPhoto", new {userId = userId, id = photo.Id}, phootToReturn);
            }
            
            return BadRequest("Could not add the photo.. ");
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int id, int userId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var user = await _repo.GetUser(userId);

            if (!user.Photos.Any( p => p.Id == id))
                return Unauthorized();
            
            var photoFromRepo = await _repo.GetPhoto(id);

            if (photoFromRepo.IsMain)
                return BadRequest("thi photo already set at main");

            var currntMainPhoto = await _repo.GetMianPhotoForUser(userId);
            currntMainPhoto.IsMain = false;
            photoFromRepo.IsMain = true; 

            if (await _repo.SaveAll())
                return NoContent();
            
            return BadRequest("could not set the main photo."); 

            
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var user = await _repo.GetUser(userId);

            if (!user.Photos.Any( p => p.Id == id))
                return Unauthorized();
            
            var photoFromRepo = await _repo.GetPhoto(id);

            if (photoFromRepo.IsMain)
                return BadRequest("You cannot delete your main photo.");

            if (photoFromRepo.PublicId != null)
            {
                var deleteParam = new DeletionParams(photoFromRepo.PublicId);
                var result = _cloudinary.Destroy(deleteParam);

                if (result.Result == "ok")
                    _repo.Delete(photoFromRepo);    
            }
            
            if (photoFromRepo.PublicId == null)
            {
                _repo.Delete(photoFromRepo);    
            } 
            

            if (await _repo.SaveAll())
                return Ok();

            return BadRequest("Failed to delete the photo"); 
        }


    }
}