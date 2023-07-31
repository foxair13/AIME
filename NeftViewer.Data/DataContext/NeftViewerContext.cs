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
        public DbSet<AspNetUsers> Users { get; set; }
        public DbSet<AspNetRoleClaims> AspNetRoleClaims { get; set; }
        public DbSet<AspNetRoles> AspNetRoles { get; set; }
        public DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public DbSet<AspNetUsers> AspNetUsers { get; set; }
        public DbSet<AspNetUserTokens> AspNetUserTokens { get; set; }
        public DbSet<Criterias> Criterias { get; set; }
        public DbSet<Customers> Customers { get; set; }
        public DbSet<IndicatorValues> IndicatorValues { get; set; }
        public DbSet<ObjectOnRoad> ObjectOnRoad { get; set; }
        public DbSet<Objects> Objects { get; set; }
        public DbSet<Roads> Roads { get; set; }
        public DbSet<Models.Action> Actions { get; set; }
        public DbSet<ActionRole> ActionRoles { get; set; }
        public NeftViewerContext(DbContextOptions<NeftViewerContext> options)
         : base(options)
        {
        }
        private readonly string _connectionString;
        public NeftViewerContext()
        {
            
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseNpgsql("Host=172.20.2.165;Port=5432;Database=NeftTest;Username=postgres;Password=7f4df451");
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetUserClaims>()
       .HasOne(u => u.AspNetUsers)
       .WithMany(uc => uc.UserClaims)
       .HasForeignKey(u => u.UserId)
       .HasPrincipalKey(u => u.Id)
       .IsRequired();            
            modelBuilder.Entity<AspNetUserClaims>()
       .HasOne(u => u.AspNetUsers)
       .WithMany(uc => uc.UserClaims)
       .HasForeignKey(u => u.UserId)
       .HasPrincipalKey(u => u.Id)
       .IsRequired();
            modelBuilder.Entity<AspNetUserTokens>().HasKey(e => new { e.UserId, e.LoginProvider, e.Name });
            modelBuilder.Entity<AspNetUserRoles>().HasKey(e => new { e.UserId, e.RoleId });
            modelBuilder.ApplyConfiguration(new AspNetUsersConfiguration());
            modelBuilder.ApplyConfiguration(new ActionConfiguration());
            modelBuilder.ApplyConfiguration(new ActionRoleConfiguration());
            OnModelCreatingPartial(modelBuilder);
           
           
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }

}
