using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Infrastructure.Persistence.EntityConfigurations
{
    public class ChatMessageEntityConfiguration : IEntityTypeConfiguration<ChatMessage>
    {
        public void Configure(EntityTypeBuilder<ChatMessage> builder)
        {
            builder.HasKey(m => m.Id);
            builder.ToTable("ChatMessages");

            builder.Property(m => m.ClientId).IsRequired();
            builder.Property(m => m.AgentId).IsRequired();
            builder.Property(m => m.Content).IsRequired();
        }
    }
}
