using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ECommerce.Application.Contracts.Redis;
using Microsoft.EntityFrameworkCore.Storage;
using StackExchange.Redis;

namespace ECommerce.Application.Services.Redis
{
    public class CacheService<T> : ICacheService<T> where T : class
    {
        private readonly StackExchange.Redis.IDatabase database;
        public CacheService(IConnectionMultiplexer connectionMultiplexer)
        {
            database = connectionMultiplexer.GetDatabase();
        }
        public Task<T> GetAsync(string key, CancellationToken ct = default)
        {
            var value = database.StringGet(key);

            var result =JsonSerializer.Deserialize<T>(value,new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
            return Task.FromResult(result);
        }

        public Task RemoveAsync(string key, CancellationToken ct = default)
        {
            database.KeyDelete(key);
            return Task.CompletedTask;
        }

        public Task<T> SetAsync(string key, T value, TimeSpan? expiration = null, CancellationToken ct = default)
        {
            database.StringSetAsync(key, JsonSerializer.Serialize(value, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }), expiration);
            return Task.FromResult(value);
        }

        public Task<T> UpdateAsynn(string key, T value, TimeSpan? expiration = null, CancellationToken ct = default)
        {
           return SetAsync(key, value, expiration, ct);       
        }
    }
}
