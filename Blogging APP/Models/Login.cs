using System.ComponentModel.DataAnnotations;

namespace Blogging_APP.Models
{
    public class Login
    {
        [Key] // Primary key
        public int Id { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public required string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        public required string Password { get; set; }

        [Required]
        public required string Role { get; set; } // Added Role property

        // Parameterless constructor to initialize required properties
        public Login()
        {
            Email = string.Empty;
            Password = string.Empty;
            Role = "User"; // Default role
        }
    }
}
