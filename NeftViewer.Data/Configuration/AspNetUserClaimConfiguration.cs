using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using NeftViewer.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Vml.Office;

namespace NeftViewer.Data.Configuration
{
    public class AspNetUserClaimConfiguration : IEntityTypeConfiguration<AspNetUserClaim>
    {

        public void Configure(EntityTypeBuilder<AspNetUserClaim> builder)
        {
            builder.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");
            builder.HasOne(u => u.AspNetUsers)
                   .WithMany(uc => uc.AspNetUserClaims)
                   .HasForeignKey(u => u.UserId)
                   .HasPrincipalKey(u => u.Id)
                   .IsRequired();
        }

    }
}
