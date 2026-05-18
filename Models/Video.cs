using System;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace EmptyMvcProject.Models
{
    public class Video
    {
        public int Id { get; set; }
        public string? Question { get; set; }
        public string? Answer { get; set; }
        public bool IsUploaded { get; set; } = false;
        public string Name { get; set; } = string.Empty;
        public string? Url { get; set; }
        public string? Description { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;

        // Foreign Key
        public int PlaylistId { get; set; }

        // Navigation property
        public Playlist? Playlist { get; set; }

        [NotMapped]
        public IFormFile? QuestionImage { get; set; }
    }
}
