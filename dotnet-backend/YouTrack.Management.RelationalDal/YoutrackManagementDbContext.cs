using Microsoft.EntityFrameworkCore;
using YouTrack.Management.Shared.Entities.Issue;

namespace YouTrack.Management.RelationalDal
{
    public class YoutrackManagementDbContext : DbContext
    {
        public YoutrackManagementDbContext(DbContextOptions<YoutrackManagementDbContext> options) : base(options)
        {
        }

        public DbSet<Assignee> Assignees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Assignee>()
                .Ignore(assignee => assignee.Competences);
            modelBuilder.Entity<Assignee>()
                .HasKey(x => new { x.Id, x.ProjectName });
        }
    }
}