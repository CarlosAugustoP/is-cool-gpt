using IsCool.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace IsCool.DI
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPostgresConnection(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            Console.WriteLine($"[DEBUG-CONNECTION] Using connection string: {connectionString}");
            services.AddDbContext<AppDbContext>(options =>
              options.UseNpgsql(connectionString));

            services.AddScoped(_ => new NpgsqlConnection(connectionString));

            return services;
        }
    }
}