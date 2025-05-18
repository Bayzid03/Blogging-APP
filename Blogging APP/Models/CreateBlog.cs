using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace Blogging_APP.Models
{
    public class CreateBlog // Corrected the typo in the class name
    {
        [Key] // Primary key
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Tags are required.")]
        [RegularExpression(@"^[a-zA-Z0-9, ]*$", ErrorMessage = "Tags must be alphanumeric and separated by commas.")]
        public string Tags { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        public string Content { get; set; }

        [NotMapped] // Exclude this property from being mapped to the database
        public IFormFile? Image { get; set; }

        // Parameterless constructor to initialize required properties
        public CreateBlog()
        {
            Title = string.Empty;
            Category = string.Empty;
            Tags = string.Empty;
            Content = string.Empty;
        }
    }
}
