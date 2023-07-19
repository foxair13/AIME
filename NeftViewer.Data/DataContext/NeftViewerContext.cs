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
            modelBuilder.Entity<AspNetRoleClaims>()
        .HasOne(r => r.AspNetRoles)
        .WithMany(rc => rc.RoleClaims)
        .HasForeignKey(r => r.RoleId)
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
            OnModelCreatingPartial(modelBuilder);
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }

}
