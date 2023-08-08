using Microsoft.EntityFrameworkCore;
using NeftViewer.Data.DataContext;
using NeftViewer.Data.Models;
using NeftViewer.MVC.FinanceViewModels;

namespace NeftViewer.MVC.FinanceModels
{

    public class FinanceViewerContext : DbContext
    {
        //public DbSet<CriteriasViewModel> Criterias { get; set; }

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
