using Microsoft.EntityFrameworkCore;
using NeftViewer.Data.Configuration;
using NeftViewer.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeftViewer.Data.DataContext
{
    public partial class NeftViewerContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public NeftViewerContext(DbContextOptions<NeftViewerContext> options)
         : base(options)
        {
        }

        protected NeftViewerContext()
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            //optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=NeftViewer;Username=postgres;Password=7f4df451");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }

}
