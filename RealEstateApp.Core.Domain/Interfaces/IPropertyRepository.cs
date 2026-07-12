using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Core.Domain.Interfaces
{
    public interface IPropertyRepository : IGenericRepository<Property>
    {
        Task ExecuteInTransactionAsync(Func<Task> operation);
    }
}
