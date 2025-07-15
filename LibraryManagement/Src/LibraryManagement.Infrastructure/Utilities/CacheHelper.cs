using Microsoft.Extensions.Caching.Memory;

namespace LibraryManagement.Infrastructure.Utilities
{
    public static class CacheHelper
    {
        public static string GenerateCacheKey(string prefix, int pageNumber, int pageSize)
        {
            return $"{prefix}_{pageNumber}_{pageSize}";
        }

        public static void InvalidateCache(IMemoryCache cache, string cacheKey)
        {
            cache.Remove(cacheKey);
        }
    }
}
