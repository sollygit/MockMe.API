using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using MockMe.Common;
using MockMe.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MockMe.API.Services
{
    public interface IMockService
    {
        Task<Product> ProductAdd(ProductRequest request);
        Task<IEnumerable<Country>> GetCountriesAsync();
        Task<IEnumerable<Product>> GetProductsAsync(int count);
    }

    public class MockService : IMockService
    {
        readonly ILogger<MockService> _logger;
        readonly IMemoryCache _cache;

        public MockService(ILogger<MockService> logger, IMemoryCache cache) =>
            (_logger, _cache) = (logger, cache);

        public async Task<Product> ProductAdd(ProductRequest request)
        {
            var data = new Product();

            try
            {
                _logger.LogInformation("ProductRequest: {request}", request.ToJson(true));

                data = MockUtil.GetData(request);
            }
            catch (Exception ex)
            {
                _logger.LogError("ProductAdd Error {ex}", ex);
            }

            return await Task.FromResult(data);
        }

        public async Task<IEnumerable<Country>> GetCountriesAsync()
        {
            // Cache Countries for 24hrs due to high page load
            return await _cache.GetOrCreateAsync("Countries", async e =>
            {
                e.SetOptions(new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
                });

                try
                {
                    var countries = await Task.FromResult(MockUtil.Countries(5));
                    return countries.OrderBy(c => c.CountryName);
                }
                catch (Exception ex)
                {
                    _logger.LogError("GetCountriesAsync Error {ex}", ex);
                }

                return Enumerable.Empty<Country>();
            });
        }

        public async Task<IEnumerable<Product>> GetProductsAsync(int count)
        {
            var products = new List<Product>();

            try
            {
                products = MockUtil.Products(count);
            }
            catch (Exception ex)
            {
                _logger.LogError("GetProductsAsync Error {ex}", ex);
            }

            return await Task.FromResult(products);
        }
    }
}
