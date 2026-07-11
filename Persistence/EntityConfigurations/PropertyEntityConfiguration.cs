using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Infrastructure.Persistence.EntityConfigurations
{
    public class PropertyEntityConfiguration : IEntityTypeConfiguration<Property>
    {
        public void Configure(EntityTypeBuilder<Property> builder)
        {
            builder.HasKey(p => p.Id);
            builder.ToTable("Properties");

            builder.Property(p => p.Code).IsRequired().HasMaxLength(6);
            builder.HasIndex(p => p.Code).IsUnique();
            builder.Property(p => p.Price).HasPrecision(18, 2);
            builder.Property(p => p.LandSize).HasPrecision(18, 2);
            builder.Property(p => p.Description).IsRequired();
            builder.Property(p => p.AgentId).IsRequired();

            builder.HasMany(p => p.Images)
                .WithOne(i => i.Property)
                .HasForeignKey(i => i.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Offers)
                .WithOne(o => o.Property)
                .HasForeignKey(o => o.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.ChatMessages)
                .WithOne(m => m.Property)
                .HasForeignKey(m => m.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Favorites)
                .WithOne(f => f.Property)
                .HasForeignKey(f => f.PropertyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
