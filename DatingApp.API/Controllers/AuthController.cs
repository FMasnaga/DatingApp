using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository repo;
        
        private readonly IConfiguration config;

        private readonly IMapper _mapper;

        public AuthController(IAuthRepository repo, IConfiguration config, IMapper mapper)
        {
            this.repo = repo;
            this.config = config;
            _mapper = mapper;
        }
        [HttpPost("register")]
        public async Task<IActionResult> register (UserDto dto){
            dto.username = dto.username.ToLower();

            if (await repo.userExists(dto.username)){
                return BadRequest ("Username has been used");
            }

            User userToCreate = _mapper.Map<User>(dto);

            var createdUser = await repo.register(userToCreate, dto.password);
            
            var userToReturn = _mapper.Map<UserForDetailDto>(createdUser);

            return CreatedAtRoute("GetUser", new {Controller = "User", id= createdUser.Id}, userToReturn);

        }
        
        [HttpPost("login")]
        public async Task<IActionResult> login (loginDto dto){

            var user = await repo.login(dto.username.ToLower(), dto.password);
            
            if (user == null){
                return Unauthorized();
            }

            var claims= new[]{
                new Claim (ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim (ClaimTypes.Name, user.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha384Signature);

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity (claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var mappedUser = _mapper.Map<UserForListDto>(user);

            return Ok (new {
                token = tokenHandler.WriteToken(token),
                mappedUser
            });
        }
        
    }
}