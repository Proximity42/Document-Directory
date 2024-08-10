using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Reflection.Metadata;

namespace Document_Directory.Server.ModelsDB
{
    public class AppDBContext : DbContext
    {
        public DbSet<Users> Users { get; set; }
        public DbSet<Groups> Groups { get; set; }
        public DbSet<NodeAccess> NodeAccess { get; set; }
        public DbSet<NodeHierarchy> NodeHierarchy { get; set; }
        public DbSet<Nodes> Nodes { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<UserGroups> UserGroups { get; set; }

        public AppDBContext(DbContextOptions<AppDBContext> options) : base (options)
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();

        }

    }
}
