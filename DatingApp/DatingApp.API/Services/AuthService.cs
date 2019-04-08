using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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
    public async Task<User> Login(string username, string password)
    {
      User matchedUser = await this._context.Users.FirstOrDefaultAsync(_user => _user.Username == username);
      if (matchedUser == null) {
        return null;
      }
      return verifyPasswordHash(password, matchedUser.PasswordHash, matchedUser.PasswordSalt) ? matchedUser : null; 
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

    public async Task<bool> UserExists(string username)
    {
      return (await this._context.Users.AnyAsync(_user => _user.Username == username));
    }

    private void createPasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
      using (HMACSHA512 hmacSha512 = new HMACSHA512()) {
        passwordSalt = hmacSha512.Key;
        passwordHash = hmacSha512.ComputeHash(Encoding.UTF8.GetBytes(password));
      };
    }

    private bool verifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt) {
      using (HMACSHA512 hmacSha512 = new HMACSHA512(passwordSalt)) { 
        var computedHash = hmacSha512.ComputeHash(Encoding.UTF8.GetBytes(password));
        var hashLength = computedHash.Length;
        for(int i=0; i<hashLength; i++) {
          if (computedHash[i] != passwordHash[i]) {
            return false;
          }
        }
      }
      return true;
    }    
  }
}
