using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Infrastructure.Persistence.EntityConfigurations
{
    public class OfferEntityConfiguration : IEntityTypeConfiguration<Offer>
    {
        public void Configure(EntityTypeBuilder<Offer> builder)
        {
            builder.HasKey(o => o.Id);
            builder.ToTable("Offers");

            builder.Property(o => o.ClientId).IsRequired();
            builder.Property(o => o.Amount).HasPrecision(18, 2);
        }
    }
}
