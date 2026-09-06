using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommerce.Application.Contracts.Redis
{
    public interface ICacheService<T> where T: class
    {
        Task<T> GetAsync(string key, CancellationToken ct = default);
        Task<T> SetAsync(string key, T value, TimeSpan? expiration = null, CancellationToken ct = default);
        Task RemoveAsync(string key, CancellationToken ct = default);
        Task<T> UpdateAsynn(string key, T value, TimeSpan? expiration = null, CancellationToken ct = default);

    }
}
