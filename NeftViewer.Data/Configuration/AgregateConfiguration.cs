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
    public class AgregateConfiguration : IEntityTypeConfiguration<Agregate>
    {
        public void Configure(EntityTypeBuilder<Agregate> builder)
        {
            builder.Property(s => s.Id).IsRequired();
            builder.Property(s => s.Name).IsRequired();
            builder.HasData(
                new Agregate { Id = 1, Name = "Финансовые" },
                new Agregate { Id = 2, Name = "Технические" }
            );
        }
    }
}
