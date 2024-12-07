using System.ComponentModel.DataAnnotations;

namespace AbjjadMicroblogging.API.Models
{
    public class PostRequest
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string OriginalImageUrl { get; set; }
        public string ResizedImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        [Required]
        [StringLength(140, ErrorMessage = "Post text must be 140 characters or fewer.")]
        public string Text { get; set; }

        [FileExtensions(Extensions = "jpg,png,webp", ErrorMessage = "Only JPG, PNG, and WebP formats are allowed.")]
        public IFormFile Image { get; set; }
    }
}
