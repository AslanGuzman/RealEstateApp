using RealEstateApp.Core.Domain.Common;

namespace RealEstateApp.Core.Domain.Entities
{
    public class PropertyType : BasicEntity<int>
    {
        public ICollection<Property>? Properties { get; set; }
    }
}
