using LafiamiAPI.Interfaces.UnitofWorks;
using LafiamiAPI.Utilities.Constants;
using LafiamiAPI.Utilities.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;

namespace LafiamiAPI.Controllers
{
    public class BaseController<T> : ControllerBase
    {
        public Type _sourceType;
        public static string GetMethodName([CallerMemberName] string name = null)
        {
            return name;
        }

        public IMemoryCache cache;
        public readonly ILogger<T> logger;
        public readonly ISystemUnitofWork systemUnitofWork;
        private readonly StringBuilder stringBuilder = new StringBuilder();


        public BaseController(IMemoryCache memoryCache, ILogger<T> logger, ISystemUnitofWork systemUnitofWork)
        {
            this.systemUnitofWork = systemUnitofWork;
            cache = memoryCache;
            this.logger = logger;
            _sourceType = GetType();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public string GenerateCacheName(params object[] values)
        {
            stringBuilder.Clear();
            stringBuilder.AppendJoin(Constants.Comma, values);
            return stringBuilder.ToString();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        private void ClearBackagroundCache()
        {
            try
            {
                var cacheListToBeRemoved = systemUnitofWork.ManageCacheService.GetQueryable(r => !r.IsCleared)
                    .Select(r => new
                    {
                        r.Id,
                        r.SourceName
                    })
                    .ToListAsync().GetAwaiter().GetResult();

                if (cacheListToBeRemoved.Any())
                {
                    foreach (var cacheObj in cacheListToBeRemoved)
                    {
                        CacheObj.ClearCache(cache, cacheObj.SourceName.Replace(Constants.Space, string.Empty));
                        systemUnitofWork.ManageCacheService.MarkedManageCacheCleared(cacheObj.Id).GetAwaiter().GetResult();
                    }

                    systemUnitofWork.SaveAsync().GetAwaiter().GetResult();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public static string CreateUrlQuery(params string[] queryParameters)
        {
            string result = string.Empty;
            foreach (string parameter in queryParameters)
            {
                if (string.IsNullOrEmpty(result))
                {
                    result = Constants.QuestionMark;
                }
                else
                {
                    result += Constants.Ampersand;
                }
                result += parameter;
            }
            return result;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public string GetUsername()
        {
            string getUsername = HttpContext.User.Claims.Where(t => t.Type == ClaimTypes.Name).Select(r => r.Value).FirstOrDefault();
            if (string.IsNullOrEmpty(getUsername))
            {
                return string.Empty;
            }

            return getUsername;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public string GetUserId()
        {
            string getUserId = HttpContext.User.Claims.Where(t => t.Type == ClaimTypes.NameIdentifier).Select(r => r.Value).FirstOrDefault();
            if (string.IsNullOrEmpty(getUserId))
            {
                return null;
            }

            return getUserId;
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        public void SavetoCache(object result, string cachename)
        {
            SavetoCache(result, cachename, (Constants.TotalHoursIn7Days));
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public void SavetoCache(object result, string cachename, double timeinhrs)
        {
            cachename = CacheObj.AppendCacheName(cachename, GetSourceName());

            // Set cache options.
            MemoryCacheEntryOptions cacheEntryOptions = new MemoryCacheEntryOptions()
                // Keep in cache for this time, reset time if accessed.
                .SetSlidingExpiration(TimeSpan.FromDays(timeinhrs));

            // Save data in cache.
            CacheObj.AddtoGlobalCache(cachename, cache);
            cache.Set(cachename, result, cacheEntryOptions);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public object GetFromCache(string cachename, bool ignoreCode = false)
        {
            ClearBackagroundCache();
            cachename = CacheObj.AppendCacheName(cachename, GetSourceName());
            cache.TryGetValue(cachename, out object obj);
            return obj;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public void ClearCache()
        {
            CacheObj.ClearCache(cache, GetSourceName());
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public string GetSourceName()
        {
            string controllerName = string.Empty;
            System.Reflection.FieldInfo[] fields = _sourceType.GetFields();
            if (fields.Length <= Constants.Zero)
            {
                controllerName = _sourceType.Name;
            }
            else
            {
                System.Reflection.FieldInfo field = fields.Where(r => r.Name == Constants.ControllerName).FirstOrDefault();
                if (field == null)
                {
                    controllerName = _sourceType.Name;
                }
                else
                {
                    controllerName = (string)field.GetValue(field);
                }
            }
            return controllerName.Replace(Constants.Space, string.Empty);
        }
    }
}
