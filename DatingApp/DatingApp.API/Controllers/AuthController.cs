using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.DTO;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AuthController: ControllerBase
  {
    private readonly IAuthService _authService;
    private const string USERNAME_ALREADY_EXIST = "Username already exist.";
    private const string UNABLE_TO_REGISTER_NEW_USER = "Unable to register new User.";

    public AuthController(IAuthService authService) {
      this._authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(RegisterUserDto registerUserDto)
    {
      registerUserDto.Username = !string.IsNullOrWhiteSpace(registerUserDto.Username) ? registerUserDto.Username.ToLower() : string.Empty; // username should be lowcased
      // does username exist ?
      if ( (await this._authService.UserExists(registerUserDto.Username)) || (registerUserDto.Username == string.Empty)) {
        return base.BadRequest(USERNAME_ALREADY_EXIST);
      }

      var newUser = new User { Username = registerUserDto.Username };
      var createdUser = await _authService.Register(newUser, registerUserDto.Password);
      if (createdUser == null) {
        return base.BadRequest(UNABLE_TO_REGISTER_NEW_USER);
      }
      
      return StatusCode(201);
    }
  }
}