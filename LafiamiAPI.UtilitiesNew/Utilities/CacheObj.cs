using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LafiamiAPI.Utilities.Utilities
{
    public static class CacheObj
    {
        public static void AddtoGlobalCache(string name, IMemoryCache cache)
        {
            GetDefaultInfo(cache, out string globalname, out List<string> cachelist);
            cachelist.Add(name);
            cache.Set(globalname, cachelist, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(30)));
        }

        private static void GetDefaultInfo(IMemoryCache cache, out string globalname, out List<string> cachelist)
        {
            globalname = "GlobalCache";
            object obj = cache.Get(globalname);
            cachelist = new List<string>();
            if (obj != null)
            {
                cachelist = (List<string>)obj;
            }
        }

        public static List<string> GetGlobalCache(IMemoryCache cache)
        {
            GetDefaultInfo(cache, out _, out List<string> cachelist);
            return cachelist;
        }

        public static string AppendCacheName(string cachename, string sourceTypeName)
        {
            return (sourceTypeName + cachename);
        }

        public static void ClearCache(IMemoryCache cache, string sourceTypeName)
        {
            string cachename = "";
            cachename = AppendCacheName(cachename, sourceTypeName);

            List<string> cachelist = GetGlobalCache(cache);
            if (cachelist != null)
            {
                List<string> removecaches = cachelist.Where(r => r.StartsWith(cachename, StringComparison.InvariantCultureIgnoreCase)).ToList();
                foreach (string item in removecaches)
                {
                    cache.Remove(item);
                }
            }
        }
    }
}
