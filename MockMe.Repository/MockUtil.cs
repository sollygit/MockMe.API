using Bogus;
using MockMe.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MockMe.Common
{
    public static class MockUtil
    {
        public static IEnumerable<CurrencyPair> CurrencyPairs = (IEnumerable<CurrencyPair>)Enum.GetValues(typeof(CurrencyPair));

        public static List<AssetTrade> Trades(int count)
        {
            var trades = new Faker<AssetTrade>()
                .RuleFor(o => o.Id, f => Guid.NewGuid().ToString())
                .RuleFor(o => o.Asset, f => {
                    var pair = new Faker().Random.ListItem((IList<CurrencyPair>)CurrencyPairs);
                    return new Asset((int)pair, pair.Description());
                })
                .RuleFor(o => o.Amount, f => decimal.Parse(f.Random.Decimal(1000).ToString("0.00")))
                .RuleFor(o => o.Expiration, f => f.Random.Number(1, 1000))
                .RuleFor(o => o.Payout, f => f.Random.Number(1, 1000))
                .RuleFor(o => o.Direction, f => f.Random.Number(0, 1))
                .Generate(count);
            return trades;
        }

        public static List<Product> Products(int count)
        {
            var products = new Faker<Product>()
                .RuleFor(o => o.Id, f => f.IndexFaker + 1)
                .RuleFor(o => o.Name, f => f.Name.FullName())
                .RuleFor(o => o.Description, f => f.Random.Words(5))
                .RuleFor(o => o.Price, f => decimal.Parse(f.Random.Decimal(1000).ToString("0.00")))
                .RuleFor(o => o.ImageUrl, f => f.Image.PicsumUrl())
                .RuleFor(o => o.Quantity, f => f.Random.Number(1, 10000))
                .Generate(count);
            return products;
        }

        public static List<Country> Countries(int count)
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

        public static MockResponse GetData(Guid submissionId)
        {
            var data = new Faker<MockResponse>()
                .RuleFor(o => o.Id, submissionId)
                .RuleFor(o => o.AccessToken, f => f.Random.Hash())
                .RuleFor(o => o.Description, f => f.Random.Words(5))
                .RuleFor(o => o.Price, f => decimal.Parse(f.Random.Decimal(1000).ToString("0.00")))
                .RuleFor(o => o.ImageUrl, f => f.Image.PicsumUrl())
                .RuleFor(o => o.Quantity, f => f.Random.Number(1, 1000));
            return data;
        }
    }
}
