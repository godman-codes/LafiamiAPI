using LafiamiAPI.Datas;
using LafiamiAPI.Datas.Models;
using LafiamiAPI.Interfaces.SystemInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace LafiamiAPI.Services.SystemServices
{
    public class ManageCacheService : RepositoryBase<ManageCacheModel, Guid>, IManageCacheService
    {
        public ManageCacheService(LafiamiContext repositoryContext)
        : base(repositoryContext)
        {
        }

        public async Task AddToManageCache(string sourceName)
        {
            //Check if source name already exist and pending 
            if (!await GetQueryable((r => (r.SourceName == sourceName) && !r.IsCleared)).AnyAsync())
            {
                ManageCacheModel cache = await GetQueryable((r => (r.SourceName == sourceName))).FirstOrDefaultAsync();
                if (cache == null)
                {
                    Add(new ManageCacheModel()
                    {
                        IsCleared = false,
                        Id = Guid.NewGuid(),
                        SourceName = sourceName,
                    });
                }
                else
                {
                    cache.IsCleared = false;
                    cache.UpdatedDate = DateTime.Now;
                }
            }
        }

        public async Task MarkedManageCacheCleared(string sourceName)
        {
            ManageCacheModel cache = await GetQueryable((r => (r.SourceName == sourceName))).FirstOrDefaultAsync();
            if (cache != null)
            {
                cache.IsCleared = true;
                cache.UpdatedDate = DateTime.Now;
            }
        }

        public async Task MarkedManageCacheCleared(Guid manageCacheId)
        {
            ManageCacheModel cache = await GetQueryable((r => (r.Id == manageCacheId))).FirstOrDefaultAsync();
            if (cache != null)
            {
                cache.IsCleared = true;
                cache.UpdatedDate = DateTime.Now;
            }
        }


    }
}
