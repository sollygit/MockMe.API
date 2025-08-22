using Bogus;
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
        Task<AssetTrade> GenerateAsync();
    }

    public class TradeRepository : ITradeRepository
    {
        static List<AssetTrade> Trades;

        public TradeRepository()
        {
            Trades ??= GenerateTrades(10);
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
            var randomTrade = GenerateTrades(1).FirstOrDefault();
            return await Task.FromResult(randomTrade);
        }

        List<AssetTrade> GenerateTrades(int count)
        {
            var currencyPairs = ((IEnumerable<CurrencyPair>)Enum.GetValues(typeof(CurrencyPair))).ToList();
            var trades = new Faker<AssetTrade>()
                .RuleFor(o => o.Id, f => Guid.NewGuid().ToString())
                .RuleFor(o => o.Asset, f => {
                    var pair = new Faker().Random.ListItem((IList<CurrencyPair>)currencyPairs);
                    return new Asset((int)pair, pair.Description());
                })
                .RuleFor(o => o.Amount, f => decimal.Parse(f.Random.Decimal(1000).ToString("0.00")))
                .RuleFor(o => o.Expiration, f => f.Random.Number(1, 1000))
                .RuleFor(o => o.Payout, f => f.Random.Number(1, 1000))
                .RuleFor(o => o.Direction, f => f.Random.Number(0, 1))
                .Generate(count);
            return trades;
        }
    }
}
