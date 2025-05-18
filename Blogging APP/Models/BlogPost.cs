using System.ComponentModel.DataAnnotations;

namespace Blogging_APP.Models
{
    public class BlogPost
    {
        [Key] // Primary key
        public int Id { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        public string Category { get; set; }

        [Required(ErrorMessage = "Content is required.")]
        public string Content { get; set; }
        public String Tags { get; set; }

        // Constructor for initializing default values

        [System.Diagnostics.CodeAnalysis.SetsRequiredMembers]
        public BlogPost()
        {
            Title = string.Empty;
            Category = string.Empty;
            Content = string.Empty;
            Tags = string.Empty;
        }
    }
}
