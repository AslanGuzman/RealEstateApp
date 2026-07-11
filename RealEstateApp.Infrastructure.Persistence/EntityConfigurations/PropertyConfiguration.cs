using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Infrastructure.Persistence.EntityConfigurations
{
    public class PropertyConfiguration : IEntityTypeConfiguration<Property>
    {
        public void Configure(EntityTypeBuilder<Property> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code)
                .IsRequired()
                .HasMaxLength(6);

            builder.HasIndex(x => x.Code)
                .IsUnique();

            builder.Property(x => x.Price)
                .IsRequired()
                .HasColumnType("decimal(18,2)"); 

            builder.Property(x => x.Size)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Description)
                .IsRequired();

            builder.HasOne(x => x.PropertyType)
                .WithMany(x => x.Properties)
                .HasForeignKey(x => x.PropertyTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SaleType)
                .WithMany(x => x.Properties)
                .HasForeignKey(x => x.SaleTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Images)
                .WithOne(x => x.Property)
                .HasForeignKey(x => x.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Offers)
                .WithOne(x => x.Property)
                .HasForeignKey(x => x.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Messages)
                .WithOne(x => x.Property)
                .HasForeignKey(x => x.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Favorites)
                .WithOne(x => x.Property)
                .HasForeignKey(x => x.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Improvements)
                .WithMany(x => x.Properties)
                .UsingEntity(j => j.ToTable("PropertyImprovements"));
        }
    }
}