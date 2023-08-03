using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeftViewer.Data.Models;

namespace NeftViewer.Data.Configuration
{
    public class AspNetRoleConfiguration : IEntityTypeConfiguration<AspNetRole>
    {
        public void Configure(EntityTypeBuilder<AspNetRole> builder)
        {
           
            builder.Property(s => s.Name).HasMaxLength(256);
            builder.Property(e => e.NormalizedName).HasMaxLength(256);
        }
    }
}
