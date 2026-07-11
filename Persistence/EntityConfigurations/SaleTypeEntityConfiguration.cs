using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Infrastructure.Persistence.EntityConfigurations
{
    public class SaleTypeEntityConfiguration : IEntityTypeConfiguration<SaleType>
    {
        public void Configure(EntityTypeBuilder<SaleType> builder)
        {
            builder.HasKey(st => st.Id);
            builder.ToTable("SaleTypes");

            builder.Property(st => st.Name).IsRequired().HasMaxLength(255);

            builder.HasMany(st => st.Properties)
                .WithOne(p => p.SaleType)
                .HasForeignKey(p => p.SaleTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
