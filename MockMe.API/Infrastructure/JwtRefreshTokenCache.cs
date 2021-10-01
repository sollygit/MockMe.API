using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MockMe.API.Infrastructure
{
    public class JwtRefreshTokenCache : IHostedService, IDisposable
    {
        private readonly ILogger<JwtRefreshTokenCache> _logger;
        private readonly IJwtAuthManager _jwtAuthManager;
        private Timer _timer;

        public JwtRefreshTokenCache(ILogger<JwtRefreshTokenCache> logger, IJwtAuthManager jwtAuthManager)
        {
            _logger = logger;
            _jwtAuthManager = jwtAuthManager;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            // Remove expired refresh tokens from cache every 24 hrs.
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromHours(24));
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _logger.LogInformation($"RemoveExpiredRefreshTokens at [{DateTime.Now}]");
            _jwtAuthManager.RemoveExpiredRefreshTokens(DateTime.Now);
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
