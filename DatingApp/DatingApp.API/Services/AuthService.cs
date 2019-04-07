using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Models;

namespace DatingApp.API.Services
{
  public class AuthService : IAuthService
  {
    private readonly DataContext _context;

    public AuthService(DataContext context)
    {
      this._context = context;
    }
    public Task<User> Login(string username, string password)
    {
      throw new System.NotImplementedException();
    }

    public async Task<User> Register(User user, string password)
    {
      byte[] passwordHash, passwordSalt;
      createPasswordHash(password, out passwordHash, out passwordSalt);
      user.PasswordHash = passwordHash;
      user.PasswordSalt = passwordSalt;

      await this._context.Users.AddAsync(user);
      await this._context.SaveChangesAsync();

      return user;
    }

    public Task<bool> UserExists(string username)
    {
      throw new System.NotImplementedException();
    }

    private void createPasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
      using (HMACSHA512 hmacSha512 = new HMACSHA512()) {
          passwordSalt = hmacSha512.Key;
          passwordHash = Encoding.UTF8.GetBytes(password);
      };
    }
  }
}
