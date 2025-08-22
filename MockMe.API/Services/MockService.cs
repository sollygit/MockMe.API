using Bogus;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using MockMe.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MockMe.API.Services
{
    public interface IMockService
    {
        IEnumerable<Country> GetCountries();
    }

    public class MockService : IMockService
    {
        readonly ILogger<MockService> _logger;
        readonly IMemoryCache _cache;

        public MockService(ILogger<MockService> logger, IMemoryCache cache) =>
            (_logger, _cache) = (logger, cache);

        public IEnumerable<Country> GetCountries()
        {
            // Cache Countries for 24hrs due to high page load
            return _cache.GetOrCreate("Countries", e =>
            {
                e.SetOptions(new MemoryCacheEntryOptions {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
                });

                try
                {
                    var countries = GenerateCountries(5);
                    return countries.OrderBy(c => c.CountryName);
                }
                catch (Exception ex)
                {
                    _logger.LogError("GetCountriesAsync Error {ex}", ex);
                }

                return Enumerable.Empty<Country>();
            });
        }

        static List<Country> GenerateCountries(int count)
        {
            var countries = new Faker<Country>()
                .RuleFor(o => o.CountryId, f => f.IndexFaker + 1)
                .RuleFor(o => o.CountryName, f => f.Address.Country())
                .RuleFor(o => o.CountryCode, f => f.Address.CountryCode())
                .Generate(count);

            if (countries.Exists(c => c.CountryName == "Australia"))
            {
                countries.Single(c => c.CountryName == "Australia").CountryCode = "AU";
            }
            else
            {
                countries.Add(new Country { CountryId = count + 1, CountryCode = "AU", CountryName = "Australia" });
            }

            return countries;
        }
    }
}
