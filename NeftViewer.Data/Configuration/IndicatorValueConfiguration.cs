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
    //public class IndicatorValueConfiguration : IEntityTypeConfiguration<IndicatorValue>
    //{
    //    public void Configure(EntityTypeBuilder<IndicatorValue> builder)
    //    {
    //        builder.ToTable("IndicatorValues");
    //        builder.HasKey(iv => iv.Id);

    //        builder.HasOne(iv => iv.Customers)
    //            .WithMany(c => c.IndicatorValues)
    //            .HasForeignKey(iv => iv.CustomerId);

    //        builder.HasOne(iv => iv.Objects)
    //            .WithMany(o => o.IndicatorValues)
    //            .HasForeignKey(iv => iv.CodeSUID);

    //        builder.HasOne(iv => iv.Criterias)
    //            .WithMany(cr => cr.IndicatorValues)
    //            .HasForeignKey(iv => iv.CriteriaId);
    //    }
    //}
}
