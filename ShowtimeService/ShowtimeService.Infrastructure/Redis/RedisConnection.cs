using StackExchange.Redis;
using Microsoft.Extensions.Configuration;

namespace ShowtimeService.Infrastructure.Redis
{
    public class RedisConnection
    {
        private readonly IConfiguration _config;
        private readonly Lazy<ConnectionMultiplexer> _lazy;

        public RedisConnection(IConfiguration config)
        {
            _config = config;
            _lazy = new Lazy<ConnectionMultiplexer>(() =>
                ConnectionMultiplexer.Connect(_config.GetConnectionString("Redis"))
            );
        }

        public ConnectionMultiplexer Multiplexer => _lazy.Value;
        public IDatabase GetDatabase() => Multiplexer.GetDatabase();
    }
}
