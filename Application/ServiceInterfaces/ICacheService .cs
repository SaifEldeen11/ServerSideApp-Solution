using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.ServiceInterfaces
{
    public interface ICacheService
    {
        Task<string?> GetAsync(string cacheKey);
        Task SetAsync(string cacheKey, object cacheValue, TimeSpan timeToLive);
        Task DeleteAsync(string cacheKey);
        Task DeleteByPatternAsync(string pattern);
    }
}
