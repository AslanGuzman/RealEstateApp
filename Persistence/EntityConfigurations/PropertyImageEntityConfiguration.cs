using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Infrastructure.Persistence.EntityConfigurations
{
    public class PropertyImageEntityConfiguration : IEntityTypeConfiguration<PropertyImage>
    {
        public void Configure(EntityTypeBuilder<PropertyImage> builder)
        {
            builder.HasKey(i => i.Id);
            builder.ToTable("PropertyImages");

            builder.Property(i => i.ImagePath).IsRequired();
        }
    }
}
