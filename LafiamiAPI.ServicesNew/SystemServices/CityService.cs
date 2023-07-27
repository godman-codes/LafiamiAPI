using LafiamiAPI.Datas;
using LafiamiAPI.Datas.Models;
using LafiamiAPI.Interfaces.SystemInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LafiamiAPI.Services.SystemServices
{
    public class CityService : RepositoryBase<CityModel, int>, ICityService
    {
        public CityService(LafiamiContext repositoryContext)
        : base(repositoryContext)
        {
        }

        public async Task<bool> DoesCityExist(int id)
        {
            return await GetQueryable((r => r.Id == id)).AnyAsync();
        }

        public async Task<bool> DoesCitiesExist(List<int> ids)
        {
            foreach (int id in ids)
            {
                if (!await DoesCityExist(id))
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> DoesCityNameExist(string name, int ignoreId = 0)
        {
            IQueryable<CityModel> queryable = GetQueryable((r => r.Name.ToLower() == name.ToLower()));
            if (ignoreId > 0)
            {
                queryable = queryable.Where(r => r.Id != ignoreId);
            }
            return await queryable.AnyAsync();
        }

        public async Task<CityModel> GetCity(int id)
        {
            return await GetQueryable((r => r.Id == id)).FirstOrDefaultAsync();
        }

        public async Task<CityModel> GetCityWithState(int id)
        {
            return await GetQueryable((r => r.Id == id)).Include(r => r.State).FirstOrDefaultAsync();
        }


        public async Task<bool> GetCityStatus(int id)
        {
            return await GetQueryable(r => (r.Id == id)).Select(r => r.Enable).SingleOrDefaultAsync();
        }
        public async Task<CityModel> GetCityByName(string name)
        {
            IQueryable<CityModel> queryable = GetQueryable((r => EF.Functions.Like(r.Name, name)));
            return await queryable.SingleOrDefaultAsync();
        }

        public async Task<bool> IsCityInUse(int id)
        {
            //if websiteId is null, more than one website is using it. 
            return await GetQueryable((r => (r.Id == id) && (r.Users.Any()))).AnyAsync();
        }

    }
}
