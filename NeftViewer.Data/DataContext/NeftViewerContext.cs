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
        public DbSet<AspNetUser> Users { get; set; }
        public DbSet<AspNetRoleClaim> AspNetRoleClaims { get; set; }
        public DbSet<AspNetRole> AspNetRoles { get; set; }
        public DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public DbSet<AspNetUserRole> AspNetUserRoles { get; set; }
        public DbSet<AspNetUser> AspNetUsers { get; set; }
        public DbSet<AspNetUserToken> AspNetUserTokens { get; set; }
        public DbSet<Criteria> Criterias { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<IndicatorValue> IndicatorValues { get; set; }
        public DbSet<ObjectOnRoad> ObjectOnRoad { get; set; }
        public DbSet<Models.Object> Objects { get; set; }
        public DbSet<Road> Roads { get; set; }
        public DbSet<Models.Action> Actions { get; set; }
        public DbSet<ActionRole> ActionRoles { get; set; }
        public NeftViewerContext(DbContextOptions<NeftViewerContext> options)
         : base(options)
        {
        }
        private readonly string _connectionString;
        public NeftViewerContext(string connectionString)
        {
            _connectionString = connectionString;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseNpgsql(_connectionString);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AspNetUserLoginConfiguration());
            modelBuilder.ApplyConfiguration(new AspNetRoleClaimConfiguration());
            modelBuilder.ApplyConfiguration(new AspNetRoleConfiguration());
            modelBuilder.ApplyConfiguration(new AspNetUserClaimConfiguration());
            modelBuilder.ApplyConfiguration(new AspNetUserTokenConfiguration());
            modelBuilder.ApplyConfiguration(new AspNetUserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new AspNetUsersConfiguration());
            modelBuilder.ApplyConfiguration(new ActionConfiguration());
            modelBuilder.ApplyConfiguration(new ActionRoleConfiguration());
            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }

}
