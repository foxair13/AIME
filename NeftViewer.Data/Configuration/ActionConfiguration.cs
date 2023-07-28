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
    public class ActionConfiguration : IEntityTypeConfiguration<Models.Action>
    {
        public void Configure(EntityTypeBuilder<Models.Action> builder)
        {
            builder.Property(s => s.Name).IsRequired();
        }
    }
}
