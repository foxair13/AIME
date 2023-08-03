using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml.Office;
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
    public class AspNetUsersConfiguration:IEntityTypeConfiguration<AspNetUser>
    {
        public void Configure(EntityTypeBuilder<AspNetUser> builder)
        {
            builder.HasIndex(e => e.NormalizedEmail, "EmailIndex");
            builder.Property(p => p.Id).IsRequired();
            builder.Property(e => e.Email).HasMaxLength(256);
            builder.Property(e => e.NormalizedEmail).HasMaxLength(256);
            builder.Property(e => e.NormalizedUserName).HasMaxLength(256);
            builder.Property(e => e.UserName).HasMaxLength(256);
        }
    }
}
