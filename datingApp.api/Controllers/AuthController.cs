using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using datingApp.api.Data;
using datingApp.api.DTOs;
using datingApp.api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;


namespace datingApp.api.Controllers
{

    [Route("api/[controller]")]
    //[ApiController]

    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper; 

        public AuthController(IAuthRepository repo, IConfiguration config, IMapper mapper)
        {
            _mapper = mapper;
            _config = config;
            _repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserForRegisterDTO userForRegisterDTO)
        {
            // will validate the request here laater... cheers. 
            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            userForRegisterDTO.Name = userForRegisterDTO.Name.ToLower();

            if (await _repo.UserExists(userForRegisterDTO.Name))
                return BadRequest("User already exists.");

            var userToCreate = _mapper.Map<User>(userForRegisterDTO);
            var createdUser = _repo.Register(userToCreate, userForRegisterDTO.Password);
            return Ok(201);
            //var userToReturn = _mapper.Map<UserForDetailDto>(createdUser);

            //return CreatedAtRoute("GetUser", new { Controller = "Users", id = createdUser.Id}, userToReturn); 
       }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserForLoginDTO userForLoginDTO)
        {

            var userFromRepo = await _repo.Login(userForLoginDTO.Username.ToLower(), userForLoginDTO.Password);

            if (userFromRepo == null)
                return Unauthorized();

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Name),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var user = _mapper.Map<UserForListDto>(userFromRepo);

            return Ok(new
            {
                token = tokenHandler.WriteToken(token), user
            });

        }






    }
}