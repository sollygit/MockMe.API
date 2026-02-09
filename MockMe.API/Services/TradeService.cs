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
    public interface ITradeService
    {
        Task<IEnumerable<AssetTrade>> GetAllAsync();
        Task<AssetTrade> GetAsync(Guid id);
        Task<AssetTrade> SaveAsync(AssetTradeViewModel entity);
        Task<AssetTrade> UpdateAsync(Guid id, AssetTradeViewModel entity);
        Task<AssetTrade> DeleteAsync(Guid id);
        Task<AssetTrade> GenerateSingleAsync();
    }

    public interface ICountryService
    {
        Task<IEnumerable<Country>> GetCountriesAsync();
    }

    public interface IAssetService
    {
        Task<IEnumerable<Asset>> GetAssetsAsync();
    }

    public class TradeService : ITradeService, IAssetService, ICountryService
    {
        readonly IMemoryCache _cache;
        readonly ITradeRepository _tradeRepository;
        readonly IEnumerable<CurrencyPair> _currencyPairs;
        readonly IEnumerable<Asset> _assets;

        public TradeService(IMemoryCache cache, ITradeRepository tradeRepository)
        {
            _cache = cache;
            _tradeRepository = tradeRepository;
            _currencyPairs = Enum.GetValues(typeof(CurrencyPair)).Cast<CurrencyPair>();
            _assets = _currencyPairs.Select(pair => new Asset((int)pair, pair.Description()));
        }

        public async Task<IEnumerable<AssetTrade>> GetAllAsync()
        {
            return await _tradeRepository.GetAllAsync();
        }

        public async Task<AssetTrade> GetAsync(Guid id)
        {
            var all = await _tradeRepository.GetAllAsync();
            return all.SingleOrDefault(o => o.Id == id.ToString());
        }

        public async Task<AssetTrade> SaveAsync(AssetTradeViewModel entity)
        {
            if (entity == null) throw new ServiceException("Entity cannot be null");

            return await _tradeRepository.SaveAsync(Mapper.Map<AssetTrade>(entity));
        }

        public async Task<AssetTrade> UpdateAsync(Guid id, AssetTradeViewModel entity)
        {
            return await _tradeRepository.UpdateAsync(id, Mapper.Map<AssetTrade>(entity));
        }

        public async Task<AssetTrade> DeleteAsync(Guid id)
        {
            return await _tradeRepository.DeleteAsync(id);
        }

        public async Task<AssetTrade> GenerateSingleAsync()
        {
            var fakeTrade = await _tradeRepository.GenerateSingleAsync();
            return await _tradeRepository.SaveAsync(fakeTrade);
        }

        public async Task<IEnumerable<Asset>> GetAssetsAsync()
        {
            // Cached Assets
            return await _cache.GetOrCreateAsync("Assets", async e =>
            {
                e.SetOptions(new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24) });
                return await Task.FromResult(_assets);
            });
        }

        public async Task<IEnumerable<Country>> GetCountriesAsync()
        {
            // Cached Countries
            return await _cache.GetOrCreate("Countries", async e => {
                e.SetOptions(new MemoryCacheEntryOptions {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
                });
                return await Task.FromResult(Constants.COUNTRIES);
            });
        }
    }
}
