using System;
using RealEstateApp.Core.Domain.Common;

namespace RealEstateApp.Core.Domain.Entities
{
    public class Offer : BaseEntity
    {
        public string ClientId { get; set; } = null!;
        public int PropertyId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Pendiente";

        public Property Property { get; set; } = null!;
    }
}