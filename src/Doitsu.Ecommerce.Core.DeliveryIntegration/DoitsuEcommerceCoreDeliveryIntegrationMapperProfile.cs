using AutoMapper;
using Doitsu.Ecommerce.Core.Abstraction.ViewModels;
using Doitsu.Ecommerce.Core.DeliveryIntegration.Common;

namespace Doitsu.Ecommerce.Core.DeliveryIntegration
{
    public class DoitsuEcommerceCoreDeliveryIntegrationMapperProfile : Profile
    {
        public DoitsuEcommerceCoreDeliveryIntegrationMapperProfile()
        {
            CreateMap<CreateOrderWithOptionViewModel, CalculateDeliveryFeesRequestModel>()
                .ForMember(d => d.Address, opt => opt.MapFrom(src => src.DeliveryAddress))
                .ForMember(d => d.District, opt => opt.MapFrom(src => src.DeliveryDistrict))
                .ForMember(d => d.Province, opt => opt.MapFrom(src => src.DeliveryCity))
                .ForMember(d => d.Ward, opt => opt.MapFrom(src => src.DeliveryWard));
        }

    }
}