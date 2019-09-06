using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DotnetAPI.Data;
using DotnetAPI.Dots;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DotnetAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private IConfiguration _config;

        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto)
        {
            userForRegisterDto.username = userForRegisterDto.username.ToLower();
            if (await _repo.UserExist(userForRegisterDto.username))
                return BadRequest("Username already exists");
            var userToCreate = new User
            {
                userName = userForRegisterDto.username,

            };
            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.password);
            return StatusCode(201);
        }

        [HttpGet("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto)
        {
            var userFromRepo = await _repo.Login(userForLoginDto.username.ToLower(), userForLoginDto.password);
            if (userFromRepo == null)
                return Unauthorized();

            var claims = new[]{
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.userName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = creds
            };

            var tokenHandle = new JwtSecurityTokenHandler();

            var token = tokenHandle.CreateToken(tokenDescriptor);

            return Ok(new { token = tokenHandle.WriteToken(token) });
        }

    }
}