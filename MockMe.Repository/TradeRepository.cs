using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MockMe.Common;
using MockMe.Model;

namespace MockMe.Repository
{
    public interface ITradeRepository
    {
        Task<IEnumerable<AssetTrade>> GetAllAsync();
        Task<AssetTrade> SaveAsync(AssetTrade entity);
        Task<AssetTrade> UpdateAsync(Guid id, AssetTrade entity);
        Task<AssetTrade> DeleteAsync(Guid id);
        Task<AssetTrade> GenerateAsync();

        List<CurrencyPair> CurrencyPairList();
    }

    public class TradeRepository : ITradeRepository
    {
        private static List<CurrencyPair> CurrencyPairs;
        private static List<AssetTrade> Trades;

        public TradeRepository()
        {
            if (CurrencyPairs == null)
            {
                CurrencyPairs = MockUtil.CurrencyPairs.ToList();
            }

            if (Trades == null)
            {
                Trades = MockUtil.Trades(10);
            }
        }

        public async Task<IEnumerable<AssetTrade>> GetAllAsync()
        {
            return await Task.FromResult(Trades);
        }

        public async Task<AssetTrade> SaveAsync(AssetTrade entity)
        {
            Trades.Add(entity);
            return await Task.FromResult(entity);
        }

        public async Task<AssetTrade> UpdateAsync(Guid id, AssetTrade entity)
        {
            var item = Trades.Single(p => p.Id == id.ToString());

            item.Asset = new Asset { Id = entity.Asset.Id, Name = entity.Asset.Name };
            item.Direction = entity.Direction;
            item.Payout = entity.Payout;
            item.Amount = entity.Amount;
            item.Expiration = entity.Expiration;

            return await Task.FromResult(item);
        }

        public async Task<AssetTrade> DeleteAsync(Guid id)
        {
            var item = Trades.Single(o => o.Id == id.ToString());
            var entry = Trades.Remove(item);
            return await Task.FromResult(item);
        }

        public async Task<AssetTrade> GenerateAsync()
        {
            var randomTrade = MockUtil.Trades(1).FirstOrDefault();
            return await Task.FromResult(randomTrade);
        }

        public List<CurrencyPair> CurrencyPairList()
        {
            return CurrencyPairs;
        }
    }
}
