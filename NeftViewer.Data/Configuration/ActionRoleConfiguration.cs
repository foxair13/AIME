using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NeftViewer.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeftViewer.Data.Configuration
{
    public class ActionRoleConfiguration : IEntityTypeConfiguration<Models.ActionRole>
    {
        public void Configure(EntityTypeBuilder<ActionRole> builder)
        {
            builder.Property(s => s.ActionId).IsRequired();
            builder.Property(s => s.RoleId).IsRequired();
            builder.HasOne(ar => ar.AspNetRoles)
        .WithMany(r => r.ActionRoles)
        .HasForeignKey(ar => ar.RoleId);
            builder.HasOne(ar => ar.Action)
        .WithMany(a => a.ActionRoles)
        .HasForeignKey(ar => ar.ActionId);
        }
    }
}
