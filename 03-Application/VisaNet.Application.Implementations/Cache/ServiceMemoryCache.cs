using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisaNet.Application.Interfaces.Cache;
using System.Runtime.Caching;

namespace VisaNet.Application.Implementations.Cache
{
    public class ServiceMemoryCache : IServiceMemoryCache
    {
        public T GetOrSet<T>(string cacheKey, int durationInMinutes, Func<T> getItemCallback) where T : class
        {
            T item = MemoryCache.Default.Get(cacheKey) as T;
            if (item == null)
            {
                item = getItemCallback();
                MemoryCache.Default.Add(cacheKey, item, DateTime.Now.AddMinutes(durationInMinutes));
            }
            return item;
        }

        public T GetOrSet<T, TId>(string cacheKeyFormat, TId id, int durationInMinutes, Func<TId, T> getItemCallback) where T : class
        {
            string cacheKey = string.Format(cacheKeyFormat, id);
            T item = MemoryCache.Default.Get(cacheKey) as T;
            if (item == null)
            {
                item = getItemCallback(id);
                MemoryCache.Default.Add(cacheKey, item, DateTime.Now.AddMinutes(durationInMinutes));
            }
            return item;
        }
    }
}
