using IsCool.Models;
using Microsoft.EntityFrameworkCore;

namespace IsCool.DB
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Chat> Chats { get; set; } = null!;
        public DbSet<PromptMessage> PromptMessages { get; set; } = null!;
    }
}