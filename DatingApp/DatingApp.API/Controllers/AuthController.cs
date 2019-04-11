using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.DTO;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class AuthController: ControllerBase
  {
    private readonly IAuthService _authService;
    private readonly IConfiguration _config;
    private const string USERNAME_ALREADY_EXIST = "Username already exist.";
    private const string UNABLE_TO_REGISTER_NEW_USER = "Unable to register new User.";
    private const string DATING_APP_TOKEN = "DATING_APP_TOKEN";
    private const int TOKEN_EXPIRY_IN_DAYS = 1;

    public AuthController(IAuthService authService, IConfiguration config) {
      this._authService = authService;
      this._config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserDto args)
    {
      args.Username = !string.IsNullOrWhiteSpace(args.Username) ? args.Username.ToLower() : string.Empty; // username should be lowcased
      // does username exist ?
      if ( (await this._authService.UserExists(args.Username)) || (args.Username == string.Empty)) {
        return base.BadRequest(USERNAME_ALREADY_EXIST);
      }

      var newUser = new User { Username = args.Username };
      var createdUser = await _authService.Register(newUser, args.Password);
      if (createdUser == null) {
        return base.BadRequest(UNABLE_TO_REGISTER_NEW_USER);
      }
      
      return StatusCode(201);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto args)
    {
      User loggedInUser = await this._authService.Login(args.Username, args.Password);

      if (loggedInUser == null) {
        return Unauthorized();
      }

      #region ** Generate JWT Auth Token **    
      // Collect user's claims
      var claims = new [] {
        new Claim(ClaimTypes.NameIdentifier, loggedInUser.Id.ToString()),
        new Claim(ClaimTypes.Name, loggedInUser.Username)
      };

      var appToken = this.getAppToken();
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appToken));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
      var tokenDescriptor = new SecurityTokenDescriptor {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.Now.AddDays(TOKEN_EXPIRY_IN_DAYS),
        SigningCredentials = creds
      };
      var tokenHandler = new JwtSecurityTokenHandler();
      var token = tokenHandler.CreateToken(tokenDescriptor);

      return Ok( new {
        token = tokenHandler.WriteToken(token)
      });

      #endregion
    }

    private string getAppToken() {
      string appToken = string.Empty;
      // TODO: When ASPNETCORE_ENVIRONMENT is Production, get app token from env variable DATING_APP_TOKEN
      // return Environment.GetEnvironmentVariable(DATING_APP_TOKEN);
      appToken = this._config.GetSection("AppSettings:Token").Value;
      return appToken;
    }
  }
}