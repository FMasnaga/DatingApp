using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{   
    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {

        private readonly IDatingRepository repo;
        private readonly IMapper mapper;
        public UsersController (IDatingRepository repo, IMapper mapper){
            this.repo = repo;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams) {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            
            userParams.UserId = userId;
            
            var userFromRepo = await this.repo.GetUser (userId);
            
            if(string.IsNullOrEmpty(userParams.Gender)) {
                userParams.Gender = userFromRepo.Gender == "male" ? "female" : "male";
            }

            var users = await this.repo.GetUsers(userParams);
            var usersToReturn = this.mapper.Map<IEnumerable<UserForListDto>>(users);
            Response.AddPagination(users.CurrentPage,users.PageSize,users.TotalCount,users.TotalPages);
            return Ok(usersToReturn);
        }

        [HttpGet("{id}",Name ="GetUser")]
        public async Task<IActionResult> GetUser(int id){
            var user = await this.repo.GetUser(id);
            var userToReturn = this.mapper.Map<UserForDetailDto>(user);
            return Ok(userToReturn);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser (int id, UpdateUserDto input){
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await this.repo.GetUser(id);

            mapper.Map (input, userFromRepo);

            if (await repo.SaveAll()) return NoContent();

            throw new Exception ($"updating user {id} fail on save"); 
        }

        [HttpPost("{id}/like/{recipientId}")]
        
        public async Task<IActionResult> GetLike (int id, int recipientId){
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();
            
            var user = await this.repo.GetUser(recipientId);

            if (user == null){
                return NotFound("user does not exist");
            }

            var like = await this.repo.GetLike(id, recipientId);
            
            if (like != null){
                return BadRequest("You already like this user");
            }

            var newLike = new Like {
                HisId = id,
                HerId = recipientId
            };

            this.repo.Add<Like>(newLike);

            if (await this.repo.SaveAll()){
                return Ok();
            }    

            return BadRequest("Cannot add like");
        }
    }
}