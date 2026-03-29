using Application.ServiceInterfaces;
using Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Application.ServiceImplementation
{
    public class CacheService(ICacheRepository _repository) : ICacheService
    {
        public async Task DeleteAsync(string cacheKey)
        {
            await _repository.DeleteAsync(cacheKey);
        }

        public async Task<string?> GetAsync(string cacheKey)
        {
            return await _repository.GetAsync(cacheKey);
        }

        public async Task SetAsync(string cacheKey, object cacheValue, TimeSpan timeToLive)
        {
            var value = JsonSerializer.Serialize(cacheValue);
            await _repository.SetAsync(cacheKey, value, timeToLive); ;
        }
        public async Task DeleteByPatternAsync(string pattern)
        {
            await _repository.DeleteByPatternAsync(pattern);
        }
    }
}
