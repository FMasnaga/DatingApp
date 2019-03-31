using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
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

        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            this.repo = repo;
            this.config = config;
        }
        [HttpPost("register")]
        public async Task<IActionResult> register (UserDto dto){
            dto.username = dto.username.ToLower();

            if (await repo.userExists(dto.username)){
                return BadRequest ("Username has been used");
            }
            
            User userToCreate = new User{
                Username = dto.username
            };

            var createdUser = await repo.register(userToCreate, dto.password);
            
            return StatusCode(201);

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

            return Ok (new {
                token = tokenHandler.WriteToken(token)
            });
        }
        
    }
}