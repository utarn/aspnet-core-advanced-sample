using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RedisWebDay3.Data;
using RedisWebDay3.Repository;

namespace RedisWebDay3.Services
{
   public class MyBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<MyBackgroundService> _logger;
        private Timer _timer = default!;
        public MyBackgroundService(IServiceScopeFactory scopeFactory,
            ILogger<MyBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;

        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _timer = new Timer(DoWorkAsync, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));
            return Task.CompletedTask;

        }

        private async void DoWorkAsync(object? state)
        {
            _timer.Change(Timeout.Infinite, Timeout.Infinite);
            using (var scope = _scopeFactory.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<ICustomerRepository>();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var cacheProvider = scope.ServiceProvider.GetRequiredService<ICacheProvider>();
                _logger.LogInformation("Start fetching from redis");
                await foreach (var customer in repository.GetPendingCustomer())
                {
                    _logger.LogInformation($"Processing customer [{customer.Id}]: {customer.Name}");
                    await context.Customers.AddAsync(customer);
                    await context.SaveChangesAsync();
                    await cacheProvider.ClearCache("current_customer");
                }
                _logger.LogInformation("Done fetching");
            }

            _timer.Change(TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
        }

        public override void Dispose()
        {
            base.Dispose();
            _timer.Dispose();

        }
    }
}
