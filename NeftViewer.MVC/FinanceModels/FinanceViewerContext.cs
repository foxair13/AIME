using Microsoft.EntityFrameworkCore;
using NeftViewer.Data.DataContext;
using NeftViewer.Data.Models;

namespace NeftViewer.MVC.FinanceModels
{

    public class FinanceViewerContext : DbContext
    {
        public DbSet<Criterias> Criterias { get; set; }

        public FinanceViewerContext(DbContextOptions<FinanceViewerContext> options)
       : base(options)
        {
        }
        private readonly string _connectionString;
        public FinanceViewerContext(string connectionString)
        {
            _connectionString = connectionString;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlServer(_connectionString);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
        }
    }
}
