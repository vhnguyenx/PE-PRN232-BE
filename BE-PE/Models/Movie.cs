using System.ComponentModel.DataAnnotations;

namespace BE_PE.Models
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Genre { get; set; }

        [Range(1, 5)]
        public int? Rating { get; set; }

        [StringLength(500)]
        public string? PosterUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
