using AutoMapper;
using Doitsu.Ecommerce.ApplicationCore.Models.DeliveryIntegration.RequestModels;
using Doitsu.Ecommerce.ApplicationCore.Models.ViewModels;

namespace Doitsu.Ecommerce.Infrastructure.AutoMapperProfiles
{
    public class DeliveryIntegrationMapperProfile : Profile
    {
        public DeliveryIntegrationMapperProfile()
        {
            CreateMap<CreateOrderWithOptionViewModel, CalculateDeliveryFeesRequestModel>()
                .ForMember(d => d.Address, opt => opt.MapFrom(src => src.DeliveryAddress))
                .ForMember(d => d.District, opt => opt.MapFrom(src => src.DeliveryDistrict))
                .ForMember(d => d.Province, opt => opt.MapFrom(src => src.DeliveryCity))
                .ForMember(d => d.Ward, opt => opt.MapFrom(src => src.DeliveryWard));
        }

    }
}