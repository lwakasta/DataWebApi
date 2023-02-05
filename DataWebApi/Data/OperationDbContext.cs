using Microsoft.EntityFrameworkCore;
using DataWebApi.Models;

namespace DataWebApi.Data
{
    public class OperationDbContext : DbContext
    {
        public DbSet<Value> Values { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<Filename> Filenames { get; set; }
        public OperationDbContext(DbContextOptions<OperationDbContext> options)
            : base(options)
        {            
            Database.EnsureCreated();
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.LogTo(Console.WriteLine);
    }
}
