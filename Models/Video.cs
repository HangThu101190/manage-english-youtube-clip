using System;

namespace EmptyMvcProject.Models
{
    public class Video
    {
        public int Id { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public bool IsUploaded { get; set; } = false;
        public string Name { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public DateTime Created { get; set; } = DateTime.UtcNow;

        // Foreign Key
        public int PlaylistId { get; set; }

        // Navigation property
        public Playlist? Playlist { get; set; }
    }
}
