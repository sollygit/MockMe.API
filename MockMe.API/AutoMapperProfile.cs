using AutoMapper;
using MockMe.Model;
using MockMe.API.ViewModels;

namespace MockMe.API
{
    public class AutoMapperProfile : Profile
    {
        public static bool IsInitialized { get; set; }

        public static void Initialize()
        {
            if (IsInitialized) return;

            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<AssetTrade, AssetTradeViewModel>()
                .ForMember(o => o.Id, map => map.MapFrom(o => o.Asset.Id))
                .ForMember(o => o.Name, map => map.MapFrom(o => o.Asset.Name))
                .ForMember(o => o.Amount, map => map.MapFrom(o => o.Amount))
                .ForMember(o => o.Payout, map => map.MapFrom(o => o.Payout))
                .ForMember(o => o.Direction, map => map.MapFrom(o => o.Direction))
                .ForMember(o => o.Expiration, map => map.MapFrom(o => o.Expiration))
                .ReverseMap();
            });

            IsInitialized = true;
        }
    }
}
