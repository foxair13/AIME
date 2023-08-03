using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using NeftViewer.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeftViewer.Data.Configuration
{
    public class AspNetRoleClaimConfiguration : IEntityTypeConfiguration<AspNetRoleClaim>
    {
            public void Configure(EntityTypeBuilder<AspNetRoleClaim> builder)
            {
                builder.Property(s => s.RoleId).IsRequired();
            }

    }
}
