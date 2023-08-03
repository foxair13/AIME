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
    public class AspNetUserLoginConfiguration : IEntityTypeConfiguration<AspNetUserLogin>
    {
        public void Configure(EntityTypeBuilder<AspNetUserLogin> builder)
        {
            builder.HasKey(e => new { e.LoginProvider, e.ProviderKey });
            builder.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");
            builder.Property(e => e.LoginProvider).HasMaxLength(128);
            builder.Property(e => e.ProviderKey).HasMaxLength(128);
        }
    }
}
