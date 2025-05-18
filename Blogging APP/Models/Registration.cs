using System.ComponentModel.DataAnnotations;

namespace Blogging_APP.Models
{
    public class Registration
    {
        [Key] // Primary key
        public int Id { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        public required string Username { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public required string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        public required string Password { get; set; }

        [Required]
        public required string Role { get; set; }

        // Parameterless constructor to initialize required properties
        public Registration()
        {
            Username = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
            Role = string.Empty;
        }
    }
}
