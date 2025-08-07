using Microsoft.EntityFrameworkCore;
using CalcUtilyServic.Models;

namespace CalcUtilyServic.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<MeterReading> Readings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string folder = AppDomain.CurrentDomain.BaseDirectory;
            string path = System.IO.Path.Combine(folder, "app.db");
            optionsBuilder.UseSqlite($"Data Source={path}");
        }

    }

}
