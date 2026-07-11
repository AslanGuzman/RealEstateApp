using RealEstateApp.Core.Domain.Common;

namespace RealEstateApp.Core.Domain.Entities
{
    public class Improvement : BasicEntity<int>
    {
        public ICollection<PropertyImprovement>? PropertyImprovements { get; set; }
    }
}
