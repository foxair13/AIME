using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NeftViewer.Data.DataContext;
using NeftViewer.MVC.Models;

namespace NeftViewer.MVC.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        public DbSet<NeftViewer.MVC.Models.ActionViewModel>? ActionViewModel { get; set; }
        public DbSet<NeftViewer.MVC.Models.ActionRoleViewModel>? ActionRoleViewModel { get; set; }
        public DbSet<NeftViewer.MVC.Models.CriteriaCalcMethodViewModel>? CriteriaCalcMethodViewModel { get; set; }
  
    }
}