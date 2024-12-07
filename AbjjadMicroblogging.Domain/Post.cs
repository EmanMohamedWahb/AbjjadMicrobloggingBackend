using System.ComponentModel.DataAnnotations;

namespace AbjjadMicroblogging.Domain
{
    public class Post
    {
        public int Id { get; set; }
        public string Username { get; set; }
        [MaxLength(140)]
        public string Text { get; set; }
        public string? OriginalImageUrl { get; set; }
        public string? ResizedTabletImageUrl { get; set; }
        public string? ResizedMobileImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
