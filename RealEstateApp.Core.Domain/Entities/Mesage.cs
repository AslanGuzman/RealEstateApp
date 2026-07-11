using RealEstateApp.Core.Domain.Common;

namespace RealEstateApp.Core.Domain.Entities
{
    public class Message : BaseEntity
    {
        public string ClientId { get; set; } = null!;
        public string AgentId { get; set; } = null!;
        public int PropertyId { get; set; }
        public string Content { get; set; } = null!;
        public DateTime Date { get; set; } = DateTime.Now;
        public string Sender { get; set; } = null!; 

        public Property Property { get; set; } = null!;
    }
}