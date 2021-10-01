using AutoMapper;
using MockMe.Model;
using MockMe.Repository;
using MockMe.API.ViewModels;
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
        Task<AssetTrade> GenerateAsync();
    }

    public class TradeService : ITradeService
    {
        private readonly ITradeRepository tradeRepository;

        public TradeService(ITradeRepository tradeRepository)
        {
            this.tradeRepository = tradeRepository;
        }

        public async Task<IEnumerable<AssetTrade>> GetAllAsync()
        {
            return await tradeRepository.GetAllAsync();
        }

        public async Task<AssetTrade> GetAsync(Guid id)
        {
            var all = await tradeRepository.GetAllAsync();
            return all.SingleOrDefault(o => o.Id == id.ToString());
        }

        public async Task<AssetTrade> SaveAsync(AssetTradeViewModel entity)
        {
            if (entity == null) throw new ServiceException("Entity cannot be null");

            return await tradeRepository.SaveAsync(Mapper.Map<AssetTrade>(entity));
        }

        public async Task<AssetTrade> UpdateAsync(Guid id, AssetTradeViewModel entity)
        {
            return await tradeRepository.UpdateAsync(id, Mapper.Map<AssetTrade>(entity));
        }

        public async Task<AssetTrade> DeleteAsync(Guid id)
        {
            return await tradeRepository.DeleteAsync(id);
        }

        public async Task<AssetTrade> GenerateAsync()
        {
            var fakeTrade = await tradeRepository.GenerateAsync();
            return await tradeRepository.SaveAsync(fakeTrade);
        }
    }
}
