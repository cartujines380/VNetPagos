using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisaNet.Application.Interfaces.Cache;

namespace VisaNet.Application.Implementations.Cache
{
    public class ServiceDatabaseCache : IServiceDataBaseCache
    {
        public T GetOrSet<T>(string cacheKey, int durationInMinutes, Func<T> getItemCallback) where T : class
        {
            throw new NotImplementedException();
        }

        public T GetOrSet<T, TId>(string cacheKeyFormat, TId id, int durationInMinutes, Func<TId, T> getItemCallback) where T : class
        {
            throw new NotImplementedException();
        }
    }
}
