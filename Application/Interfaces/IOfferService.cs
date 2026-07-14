using RealEstateApp.Core.Application.Dtos.Common;
using RealEstateApp.Core.Application.Dtos.Offer;

namespace RealEstateApp.Core.Application.Interfaces
{
    public interface IOfferService : IGenericService<OfferDto>
    {
        Task<OperationResponseDto> CreateOfferAsync(SaveOfferDto dto);
        Task<OperationResponseDto> AcceptOfferAsync(int offerId, string agentId);
        Task<OperationResponseDto> RejectOfferAsync(int offerId, string agentId);
        Task<List<OfferDto>> GetByPropertyAsync(int propertyId, string? clientId = null);
        Task<List<OfferClientSummaryDto>> GetClientSummariesAsync(int propertyId);
        Task<bool> HasPendingOfferAsync(int propertyId, string clientId);
        Task<bool> HasAcceptedOfferAsync(int propertyId);
    }
}
