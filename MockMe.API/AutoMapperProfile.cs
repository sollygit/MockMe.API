using AutoMapper;
using MockMe.Model;
using MockMe.API.ViewModels;

namespace MockMe.API
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Asset, AssetViewModel>()
                .ForMember(o => o.Id, map => map.MapFrom(o => o.Id))
                .ForMember(o => o.Name, map => map.MapFrom(o => o.Name))
                .ReverseMap();

            CreateMap<AssetTrade, AssetTradeViewModel>()
                .ForMember(o => o.Asset, map => map.MapFrom(o => o.Asset))
                .ForMember(o => o.Amount, map => map.MapFrom(o => o.Amount))
                .ForMember(o => o.Payout, map => map.MapFrom(o => o.Payout))
                .ForMember(o => o.Direction, map => map.MapFrom(o => o.Direction))
                .ForMember(o => o.Expiration, map => map.MapFrom(o => o.Expiration))
                .ReverseMap();
        }
    }
}
