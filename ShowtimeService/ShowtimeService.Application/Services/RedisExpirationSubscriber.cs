using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShowtimeService.Application.Interfaces;
using StackExchange.Redis;

public class RedisExpirationSubscriber : BackgroundService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly IServiceProvider _provider;
    private readonly IDatabase _db;
    private readonly TimeSpan _pollInterval = TimeSpan.FromSeconds(30);

    public RedisExpirationSubscriber(IConnectionMultiplexer redis, IServiceProvider provider)
    {
        _redis = redis;
        _provider = provider;
        _db = redis.GetDatabase();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _provider.CreateScope();
                var notifier = scope.ServiceProvider.GetRequiredService<ISeatHubNotifier>();
                await CheckExpiredLocks(notifier);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            await Task.Delay(_pollInterval, stoppingToken);
        }
    }

    private async Task CheckExpiredLocks(ISeatHubNotifier notifier)
    {
        var server = _redis.GetServer(_redis.GetEndPoints()[0]);
        var keys = server.Keys(pattern: "showtime:*:seat:*").ToArray();

        foreach (var key in keys)
        {
            var ttl = await _db.KeyTimeToLiveAsync(key);
            if (!ttl.HasValue || ttl.Value <= TimeSpan.Zero)
            {
                var parts = key.ToString().Split(':');
                if (parts.Length < 4) continue;
                if (!Guid.TryParse(parts[1], out var showtimeId)) continue;

                var seatNumber = parts[3];
                await notifier.NotifySeatUpdated(showtimeId, seatNumber, false, "");
                await _db.KeyDeleteAsync(key);
            }
        }
    }
}
