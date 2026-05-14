using System.Collections.Generic;

namespace EmptyMvcProject.Models
{
    public class Playlist
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;

        // Navigation property
        public ICollection<Video> Videos { get; set; } = new List<Video>();
    }
}
