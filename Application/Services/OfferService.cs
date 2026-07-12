using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.Dtos.Common;
using RealEstateApp.Core.Application.Dtos.Offer;
using RealEstateApp.Core.Application.Interfaces;
using RealEstateApp.Core.Domain.Common.Enums;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Interfaces;

namespace RealEstateApp.Core.Application.Services
{
    public class OfferService : GenericService<Offer, OfferDto>, IOfferService
    {
        private readonly IOfferRepository _offerRepository;
        private readonly IPropertyRepository _propertyRepository;
        private readonly IMapper _mapper;

        public OfferService(IOfferRepository offerRepository, IPropertyRepository propertyRepository, IMapper mapper)
            : base(offerRepository, mapper)
        {
            _offerRepository = offerRepository;
            _propertyRepository = propertyRepository;
            _mapper = mapper;
        }

        public async Task<OperationResponseDto> CreateOfferAsync(SaveOfferDto dto)
        {
            if (dto.Amount <= 0)
            {
                return Error("El monto de la oferta debe ser mayor que cero");
            }

            var property = await _propertyRepository.GetById(dto.PropertyId);

            if (property == null || property.Status != PropertyStatus.Available)
            {
                return Error("La propiedad no está disponible para recibir ofertas");
            }

            var propertyOffers = _offerRepository.GetAllQuery().Where(o => o.PropertyId == dto.PropertyId);

            if (await propertyOffers.AnyAsync(o => o.Status == OfferStatus.Accepted))
            {
                return Error("La propiedad ya tiene una oferta aceptada");
            }

            if (await propertyOffers.AnyAsync(o => o.ClientId == dto.ClientId && o.Status == OfferStatus.Pending))
            {
                return Error("Ya tiene una oferta pendiente sobre esta propiedad");
            }

            await _offerRepository.AddAsync(new Offer
            {
                Id = 0,
                PropertyId = dto.PropertyId,
                ClientId = dto.ClientId,
                Amount = dto.Amount
            });

            return Success();
        }

        public async Task<OperationResponseDto> AcceptOfferAsync(int offerId, string agentId)
        {
            var offer = await _offerRepository.GetById(offerId);

            if (offer == null || offer.Status != OfferStatus.Pending)
            {
                return Error("La oferta no existe o no está pendiente");
            }

            var property = await _propertyRepository.GetById(offer.PropertyId);

            if (property == null || property.AgentId != agentId)
            {
                return Error("La propiedad no pertenece al agente autenticado");
            }

            if (property.Status != PropertyStatus.Available)
            {
                return Error("La propiedad ya no está disponible");
            }

            var hasAccepted = await _offerRepository.GetAllQuery()
                .AnyAsync(o => o.PropertyId == property.Id && o.Status == OfferStatus.Accepted);

            if (hasAccepted)
            {
                return Error("La propiedad ya tiene una oferta aceptada");
            }

            await _propertyRepository.ExecuteInTransactionAsync(async () =>
            {
                offer.Status = OfferStatus.Accepted;
                await _offerRepository.UpdateAsync(offer.Id, offer);

                var pendingOffers = await _offerRepository.GetAllQuery()
                    .Where(o => o.PropertyId == property.Id && o.Id != offer.Id && o.Status == OfferStatus.Pending)
                    .ToListAsync();

                foreach (var pending in pendingOffers)
                {
                    pending.Status = OfferStatus.Rejected;
                    await _offerRepository.UpdateAsync(pending.Id, pending);
                }

                property.Status = PropertyStatus.Sold;
                await _propertyRepository.UpdateAsync(property.Id, property);
            });

            return Success();
        }

        public async Task<OperationResponseDto> RejectOfferAsync(int offerId, string agentId)
        {
            var offer = await _offerRepository.GetById(offerId);

            if (offer == null || offer.Status != OfferStatus.Pending)
            {
                return Error("La oferta no existe o no está pendiente");
            }

            var property = await _propertyRepository.GetById(offer.PropertyId);

            if (property == null || property.AgentId != agentId)
            {
                return Error("La propiedad no pertenece al agente autenticado");
            }

            offer.Status = OfferStatus.Rejected;
            await _offerRepository.UpdateAsync(offer.Id, offer);

            return Success();
        }

        public async Task<List<OfferDto>> GetByPropertyAsync(int propertyId, string? clientId = null)
        {
            var query = _offerRepository.GetAllQuery().Where(o => o.PropertyId == propertyId);

            if (!string.IsNullOrWhiteSpace(clientId))
            {
                query = query.Where(o => o.ClientId == clientId);
            }

            var offers = await query.OrderByDescending(o => o.OfferDate).ToListAsync();

            return _mapper.Map<List<OfferDto>>(offers);
        }

        private static OperationResponseDto Success()
        {
            return new OperationResponseDto();
        }

        private static OperationResponseDto Error(string message)
        {
            return new OperationResponseDto { HasError = true, Error = message };
        }
    }
}
