using MockMe.Common;
using MockMe.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockMe.Repository
{
    public interface ITradeRepository
    {
        Task<IEnumerable<AssetTrade>> GetAllAsync();
        Task<AssetTrade> SaveAsync(AssetTrade entity);
        Task<AssetTrade> UpdateAsync(Guid id, AssetTrade entity);
        Task<AssetTrade> DeleteAsync(Guid id);
        Task<AssetTrade> GenerateSingleAsync();
    }

    public class TradeRepository : ITradeRepository
    {
        readonly IEnumerable<CurrencyPair> _currencyPairs;
        readonly IEnumerable<Asset> _assets;
        readonly Random _random = new Random();
        readonly List<AssetTrade> _trades;

        public TradeRepository()
        {
            _currencyPairs = Enum.GetValues(typeof(CurrencyPair)).Cast<CurrencyPair>();
            _assets = _currencyPairs.Select(pair => new Asset((int)pair, pair.Description()));
            _trades ??= GenerateTrades(10);
        }

        public async Task<IEnumerable<AssetTrade>> GetAllAsync()
        {
            return await Task.FromResult(_trades);
        }

        public async Task<AssetTrade> SaveAsync(AssetTrade entity)
        {
            _trades.Add(entity);
            return await Task.FromResult(entity);
        }

        public async Task<AssetTrade> UpdateAsync(Guid id, AssetTrade entity)
        {
            var item = _trades.Single(p => p.Id == id.ToString());

            item.Asset = new Asset { Id = entity.Asset.Id, Name = entity.Asset.Name };
            item.Direction = entity.Direction;
            item.Payout = entity.Payout;
            item.Amount = entity.Amount;
            item.Expiration = entity.Expiration;

            return await Task.FromResult(item);
        }

        public async Task<AssetTrade> DeleteAsync(Guid id)
        {
            var item = _trades.Single(o => o.Id == id.ToString());
            var entry = _trades.Remove(item);
            return await Task.FromResult(item);
        }

        public async Task<AssetTrade> GenerateSingleAsync()
        {
            var asset = _assets.ElementAt(_random.Next(0, _assets.Count()));
            var randomTrade = GenerateTrades(1, asset).FirstOrDefault();
            return await Task.FromResult(randomTrade);
        }

        List<AssetTrade> GenerateTrades(int count ,Asset asset = null)
        {
            var assetCount = _assets.Count();
            return Enumerable.Range(0, count)
                .Select(i => new AssetTrade 
                {
                    Asset = asset ?? _assets.ElementAt(i % _assets.Count()), // Cycles on assets
                    Amount = Math.Round(_random.Next(100, 100000) * 0.01m, 2),
                    Expiration = _random.Next(1, 1000),
                    Payout = _random.Next(1, 1000),
                    Direction = _random.Next(0, 2)
                })
                .ToList();
        }
    }
}
