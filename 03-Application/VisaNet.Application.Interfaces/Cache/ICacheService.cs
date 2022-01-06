using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisaNet.Application.Interfaces.Cache
{
    public interface ICacheService
    {
        T GetOrSet<T>(string cacheKey, int durationInMinutes, Func<T> getItemCallback) where T : class;
        T GetOrSet<T, TId>(string cacheKeyFormat, TId id, int durationInMinutes, Func<TId, T> getItemCallback) where T : class;
    }

    public interface IServiceMemoryCache : ICacheService
    { }

    public interface IServiceDataBaseCache: ICacheService
    { }

    public interface IServiceCloudCache : ICacheService
    { }
}
