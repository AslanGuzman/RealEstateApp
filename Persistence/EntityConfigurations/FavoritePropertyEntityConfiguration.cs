using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Infrastructure.Persistence.EntityConfigurations
{
    public class FavoritePropertyEntityConfiguration : IEntityTypeConfiguration<FavoriteProperty>
    {
        public void Configure(EntityTypeBuilder<FavoriteProperty> builder)
        {
            builder.HasKey(f => new { f.ClientId, f.PropertyId });
            builder.ToTable("FavoriteProperties");
        }
    }
}
