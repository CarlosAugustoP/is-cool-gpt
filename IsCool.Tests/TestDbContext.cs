using Microsoft.EntityFrameworkCore;
using IsCool.DB;

namespace IsCool.Tests
{
    public class TestAppDbContext : AppDbContext
    {
        // Construtor necessário para passar as opções do In-Memory Database para a classe base
        public TestAppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}