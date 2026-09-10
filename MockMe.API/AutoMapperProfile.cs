using AutoMapper;
using MockMe.API.ViewModels;
using MockMe.Model;

namespace MockMe.API
{
    public class AutoMapperProfile : Profile
    {
        public static bool IsInitialized { get; set; }

        public AutoMapperProfile()
        {
            if (IsInitialized) return;

            CreateMap<AssetTrade, AssetTradeViewModel>()
                .ForMember(o => o.Id, map => map.MapFrom(o => o.Asset.Id))
                .ForMember(o => o.Name, map => map.MapFrom(o => o.Asset.Name))
                .ForMember(o => o.Amount, map => map.MapFrom(o => o.Amount))
                .ForMember(o => o.Payout, map => map.MapFrom(o => o.Payout))
                .ForMember(o => o.Direction, map => map.MapFrom(o => o.Direction))
                .ForMember(o => o.Expiration, map => map.MapFrom(o => o.Expiration))
                .ReverseMap();

            IsInitialized = true;
        }
    }
}
