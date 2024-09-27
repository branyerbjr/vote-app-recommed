using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;
using VoteAppBackend.Data;
using VoteAppBackend.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.DependencyInjection;

namespace VoteAppBackend.Services
{
    public class VoteProcessingService : BackgroundService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IServiceProvider _serviceProvider;

        public VoteProcessingService(IConnectionMultiplexer redis, IServiceProvider serviceProvider)
        {
            _redis = redis;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var db = _redis.GetDatabase();

            while (!stoppingToken.IsCancellationRequested)
            {
                var data = await db.ListLeftPopAsync("votes");
                if (!data.IsNullOrEmpty)
                {
                    var vote = JsonConvert.DeserializeObject<Vote>(data);

                    // Guardar el voto en la base de datos
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        context.Votes.Add(vote);
                        await context.SaveChangesAsync();
                    }
                }
                else
                {
                    // Esperar antes de intentar de nuevo
                    await Task.Delay(1000, stoppingToken);
                }
            }
        }
    }
}
