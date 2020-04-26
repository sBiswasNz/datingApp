using System.Threading.Tasks;
using datingApp.api.Data;
using datingApp.api.DTOs;
using datingApp.api.Models;
using Microsoft.AspNetCore.Mvc; 

namespace datingApp.api.Controllers
{

    [Route("api/[controller]")]
    //[ApiController]

    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;

        public AuthController(IAuthRepository repo)
        {
            this._repo = repo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register( [FromBody] UserForRegisterDTO userForRegisterDTO )
        { 
            // will validate the request here laater... cheers. 
            if(!ModelState.IsValid)
                return BadRequest(ModelState); 


            userForRegisterDTO.username = userForRegisterDTO.username.ToLower();

            if (await _repo.UserExists(userForRegisterDTO.username))
                return BadRequest("User already exists.") ;
            
            var userToCreate = new User
            {
                Name = userForRegisterDTO.username
            };

            var cretedUser = _repo.Register(userToCreate, userForRegisterDTO.password );
            
            return StatusCode(201); 
        }
    }
}