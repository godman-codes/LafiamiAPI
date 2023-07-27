using LafiamiAPI.Datas.Models;
using System;
using System.Threading.Tasks;

namespace LafiamiAPI.Interfaces.SystemInterfaces
{
    public interface IManageCacheService : IRepositoryBase<ManageCacheModel, Guid>
    {
        Task AddToManageCache(string sourceName);
        Task MarkedManageCacheCleared(string sourceName);
        Task MarkedManageCacheCleared(Guid manageCacheId);
    }
}
