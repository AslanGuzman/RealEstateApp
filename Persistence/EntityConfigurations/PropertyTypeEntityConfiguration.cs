using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Infrastructure.Persistence.EntityConfigurations
{
    public class PropertyTypeEntityConfiguration : IEntityTypeConfiguration<PropertyType>
    {
        public void Configure(EntityTypeBuilder<PropertyType> builder)
        {
            builder.HasKey(pt => pt.Id);
            builder.ToTable("PropertyTypes");

            builder.Property(pt => pt.Name).IsRequired().HasMaxLength(255);

            builder.HasMany(pt => pt.Properties)
                .WithOne(p => p.PropertyType)
                .HasForeignKey(p => p.PropertyTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
