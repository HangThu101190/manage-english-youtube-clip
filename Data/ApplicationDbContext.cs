using Microsoft.EntityFrameworkCore;
using EmptyMvcProject.Models;

namespace EmptyMvcProject.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<Video> Videos { get; set; }
    }
}
