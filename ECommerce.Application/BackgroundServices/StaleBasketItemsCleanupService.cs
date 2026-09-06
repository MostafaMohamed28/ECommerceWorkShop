using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Domain.Contract.Baskets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.BackgroundServices
{
    public class StaleBasketItemsCleanupService: BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<StaleBasketItemsCleanupService> _logger;
        private static readonly TimeSpan StaleThreshold = TimeSpan.FromDays(7);
        private static readonly TimeSpan RunInterval = TimeSpan.FromHours(24);

        public StaleBasketItemsCleanupService(
            IServiceScopeFactory scopeFactory,
            ILogger<StaleBasketItemsCleanupService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var timer = new PeriodicTimer(RunInterval);

            // شغّلها مرة فورًا عند بدء التطبيق، بعدين كل 24 ساعة
            do
            {
                await RunCleanupAsync(stoppingToken);
            }
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }

        private async Task RunCleanupAsync(CancellationToken ct)
        {
            var startedAt = DateTime.UtcNow;
            _logger.LogInformation("Stale basket items cleanup started at {Time}", startedAt);

            try
            {
                using var scope = _scopeFactory.CreateScope();
                var writeRepository = scope.ServiceProvider.GetRequiredService<IBasketWriteRepository>();

                var cutoffDate = DateTime.UtcNow.Subtract(StaleThreshold);
                var deletedCount = await writeRepository.RemoveStaleItemsAsync(cutoffDate, ct);

                _logger.LogInformation(
                    "Stale basket items cleanup finished. Removed {Count} items. Duration: {Duration}ms",
                    deletedCount, (DateTime.UtcNow - startedAt).TotalMilliseconds);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested)
            {
                _logger.LogInformation("Stale basket items cleanup was cancelled due to shutdown.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred during stale basket items cleanup.");
            }
        }
    }
}
