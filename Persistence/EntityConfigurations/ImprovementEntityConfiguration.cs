using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Infrastructure.Persistence.EntityConfigurations
{
    public class ImprovementEntityConfiguration : IEntityTypeConfiguration<Improvement>
    {
        public void Configure(EntityTypeBuilder<Improvement> builder)
        {
            builder.HasKey(i => i.Id);
            builder.ToTable("Improvements");

            builder.Property(i => i.Name).IsRequired().HasMaxLength(255);
        }
    }
}
