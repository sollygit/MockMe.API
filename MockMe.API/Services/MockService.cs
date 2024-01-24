using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MockMe.Common;
using MockMe.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MockMe.API.Services
{
    public interface IMockService
    {
        Task<Product> ProductAdd(ProductRequest request);
        Task<IEnumerable<Country>> GetCountriesAsync();
        Task<IEnumerable<Product>> GetProductsAsync(int count);
        Task<string> RunExeAsync(string filename);


        public class MockService : IMockService
        {
            readonly ILogger<MockService> _logger;
            readonly IMemoryCache _cache;
            readonly IConfiguration _configuration;

            public MockService(ILogger<MockService> logger, IMemoryCache cache, IConfiguration configuration) =>
                (_logger, _cache, _configuration) = (logger, cache, configuration);

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

            public async Task<string> RunExeAsync(string filename)
            {
                using var process = new Process
                {
                    StartInfo = {
                    FileName = _configuration["ExecFileName"], // MockMe.CMD.exe
                    Arguments = $"{filename}",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true }
                };
                // Print the current working directory information
                // process.StartInfo.FileName = @"cmd.exe";
                // process.StartInfo.Arguments = @"/c dir";

                process.OutputDataReceived += (_, data) => Console.WriteLine(data.Data);
                process.ErrorDataReceived += (_, data) => Console.WriteLine(data.Data);

                Console.WriteLine("Start Process");
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                var exited = process.WaitForExit(1000 * 5); // (Optional) wait up to 5 seconds
                Console.WriteLine($"Exit {exited}");

                return await Task.FromResult($"{filename} success");
            }
        }
    }
}
