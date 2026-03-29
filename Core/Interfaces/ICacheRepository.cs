using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ICacheRepository
    {
        Task<string?> GetAsync(string cacheKey);
        Task SetAsync(string cacheKey, string cacheValue, TimeSpan timeToLive);
        Task DeleteAsync(string cacheKey);
        Task DeleteByPatternAsync(string pattern);
    }
    
}
