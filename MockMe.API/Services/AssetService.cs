using AutoMapper;
using Microsoft.Extensions.Caching.Memory;
using MockMe.API.ViewModels;
using MockMe.Common;
using MockMe.Model;
using MockMe.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockMe.API.Services
{
    public interface IAssetService
    {
        Task<IEnumerable<AssetViewModel>> GetAllAsync();
    }

    public class AssetService : IAssetService
    {
        private readonly IMemoryCache cache;
        private readonly ITradeRepository tradeRepository;

        public AssetService(IMemoryCache cache, ITradeRepository tradeRepository)
        {
            this.cache = cache;
            this.tradeRepository = tradeRepository;
        }

        public async Task<IEnumerable<AssetViewModel>> GetAllAsync()
        {
            return await cache.GetOrCreateAsync("AssetList", async e => 
            {
                // Cache Assets
                e.SetOptions(new MemoryCacheEntryOptions{ AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1) });

                var currencyPairs = tradeRepository.CurrencyPairList();
                var result = currencyPairs.Select(pair => 
                    Mapper.Map<AssetViewModel>(new Asset((int)pair, pair.Description())));
                return await Task.FromResult(result);
            });
        }
    }
}
