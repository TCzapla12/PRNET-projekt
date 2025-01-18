using Microsoft.EntityFrameworkCore;
using grpc_hello_world.Models;
using System.Collections.Generic;

namespace grpc_hello_world
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Animal> Animals { get; set; }
        
        public DbSet<Opinion> Opinions { get; set; }
    }
}
