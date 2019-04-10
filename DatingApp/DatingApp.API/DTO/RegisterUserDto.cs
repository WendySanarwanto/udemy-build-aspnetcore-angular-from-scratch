using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.DTO
{
    public class RegisterUserDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(64, MinimumLength = 4, ErrorMessage = "You must specify password between 4 and 64 characters.")]
        public string Password { get; set; }
    }
}