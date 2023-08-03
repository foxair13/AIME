using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using NeftViewer.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.Emit;

namespace NeftViewer.Data.Configuration
{
    public class AspNetUserRoleConfiguration : IEntityTypeConfiguration<AspNetUserRole>
    {
        public void Configure(EntityTypeBuilder<AspNetUserRole> builder)
        {
            builder.HasKey(e => new { e.UserId, e.RoleId });
            builder.Property(s => s.UserId).IsRequired();
            builder.Property(s => s.RoleId).IsRequired();
        }
    }
}
