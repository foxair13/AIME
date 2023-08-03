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
    public class AspNetUserTokenConfiguration : IEntityTypeConfiguration<AspNetUserToken>
    {
        public void Configure(EntityTypeBuilder<AspNetUserToken> builder)
        {
            builder.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });
            builder.Property(e => e.LoginProvider).HasMaxLength(128);
            builder.Property(e => e.Name).HasMaxLength(128);

        }
    }
}
