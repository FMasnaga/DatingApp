using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository repo;
        private readonly IMapper mapper;
        private readonly IOptions<CloudinarySettings> cloudSetting;

        private readonly Cloudinary cloudinary;
        
        public PhotosController (IDatingRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudSetting){
            this.repo = repo;
            this.mapper = mapper;
            this.cloudSetting = cloudSetting;

            Account acc = new Account (
            cloudSetting.Value.CloudName,
            cloudSetting.Value.ApiKey,
            cloudSetting.Value.ApiSecret
            );

            cloudinary = new Cloudinary (acc);
        }

        [HttpGet("{id}", Name ="GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id) {
            var photoFromRepo = await repo.GetPhoto(id);
            var photo = mapper.Map<PhotoForReturnDto>(photoFromRepo);
            return Ok(photo);

        }

        [HttpPost]
        public async Task<IActionResult> AddPhoto (int userId, [FromForm]PhotoForCreationDto photoDto){
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await this.repo.GetUser(userId);

            var file = photoDto.File;

            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream()){
                    var uploadParams = new ImageUploadParams(){
                        File = new FileDescription (file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500)
                        .Crop("fill").Gravity("face")
                    };

                    uploadResult = cloudinary.Upload(uploadParams);

                }
            }

            photoDto.url = uploadResult.Uri.ToString();
            photoDto.publicId = uploadResult.PublicId;

            var photo = this.mapper.Map<Photo>(photoDto);
            
            if (!userFromRepo.photos.Any(u => u.IsMain)) 
                photo.IsMain = true;

            userFromRepo.photos.Add(photo);
            
            if (await repo.SaveAll()){
                var photoToReturn= mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new {id = photo.Id}, photoToReturn);
            }


            return BadRequest("Could not add the phto");
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult>  setMainPhotos (int userId, int id){
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var user = await this.repo.GetUser(userId);

            if (!user.photos.Any(p => p.Id == id))
                return Unauthorized();
            
            var photoFromRepo = await repo.GetPhoto(id);

            if (photoFromRepo.IsMain)
                return BadRequest("photo is already set as main");
            
            var currentMainPhoto = await repo.GetMainPhoto(userId);
            
            currentMainPhoto.IsMain = false;

            photoFromRepo.IsMain = true;

            if (await repo.SaveAll())
                return NoContent();

            return BadRequest("cannot save photo as Main");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> deletePhoto (int userId, int id){
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var user = await this.repo.GetUser(userId);

            if (!user.photos.Any(p => p.Id == id))
                return Unauthorized();
            
            var photoFromRepo = await repo.GetPhoto(id);

            if (photoFromRepo.IsMain)
                return BadRequest("Can't Delete Main Photo");

            var publicId = photoFromRepo.PublicId;

            if (publicId != null){
                DeletionParams param = new DeletionParams(publicId);
                
                var result = cloudinary.Destroy(param);

                if (result.Result == "ok"){
                    repo.Delete(photoFromRepo);
                } 
            }

            if (publicId == null){
                repo.Delete(photoFromRepo);
            }

            if (await repo.SaveAll()){
                return Ok();
            }

            return BadRequest ("Can't Delete the photo");
        }
    }
}