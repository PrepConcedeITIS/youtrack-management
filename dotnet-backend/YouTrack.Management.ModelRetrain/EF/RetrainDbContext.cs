using Microsoft.EntityFrameworkCore;
using YouTrack.Management.ModelRetrain.Entities;

namespace YouTrack.Management.ModelRetrain.EF
{
    public class RetrainDbContext: DbContext
    {
        public RetrainDbContext(DbContextOptions<RetrainDbContext> options) : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }
    }
}