using Blogging_APP.Models;
using Microsoft.EntityFrameworkCore;

namespace Blogging_APP.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Registration> Registrations { get; set; }
        public DbSet<Login> Logins { get; set; }
        public DbSet<BlogPost> Blogs { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
