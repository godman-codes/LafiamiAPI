using LafiamiAPI.Datas;
using LafiamiAPI.Datas.Models;
using LafiamiAPI.Interfaces.SystemInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LafiamiAPI.Services.SystemServices
{
    public class CountryService : RepositoryBase<CountryModel, int>, ICountryService
    {
        public CountryService(LafiamiContext repositoryContext)
        : base(repositoryContext)
        {
        }
        public async Task<int> GetCountryByCity(int cityid)
        {
            return await GetQueryable((r => r.States.Any(t => t.Cities.Any(j => j.Id == cityid))))
                .Select(r => r.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetCountryByState(int stateid)
        {
            return await GetQueryable((r => r.States.Any(t => t.Id == stateid)))
                .Select(r => r.Id)
                .FirstOrDefaultAsync();
        }



        public IQueryable<CountryModel> GetCountryQueryable(int page, int pageSize, Expression<Func<CountryModel, bool>> where)
        {
            IQueryable<CountryModel> queryable = GetQueryable(where);
            if (pageSize > 0)
            {
                queryable = queryable.OrderByDescending(r => r.CreatedDate)
                    .Skip(page * pageSize)
                    .Take(pageSize);
            }

            return queryable;
        }



        public async Task<bool> IsCountryInUse(int id)
        {
            //if websiteId is null, more than one website is using it. 
            return await GetQueryable((r => (r.Id == id) && (r.Users.Any() || r.Orders.Any(t => !t.IsDeleted)))).AnyAsync();
        }

        public async Task<bool> GetCountryStatus(int id)
        {
            return await GetQueryable(r => (r.Id == id)).Select(r => r.Enable).SingleOrDefaultAsync();
        }

        public async Task<CountryModel> GetCountryByName(string name)
        {
            IQueryable<CountryModel> queryable = GetQueryable((r => EF.Functions.Like(r.Name, name)));
            return await queryable.SingleOrDefaultAsync();
        }

        public async Task<bool> DoesCountryExist(int id)
        {
            return await GetQueryable((r => r.Id == id)).AnyAsync();
        }
        public async Task<bool> DoesCountriesExist(List<int> ids)
        {
            foreach (int id in ids)
            {
                if (!await DoesCountryExist(id))
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> DoesCountryNameExist(string name, int ignoreId = 0)
        {
            IQueryable<CountryModel> queryable = GetQueryable((r => r.Name.ToLower() == name.ToLower()));
            if (ignoreId > 0)
            {
                queryable = queryable.Where(r => r.Id != ignoreId);
            }
            return await queryable.AnyAsync();
        }

        public async Task<CountryModel> GetCountry(int id)
        {
            return await GetQueryable((r => r.Id == id)).FirstOrDefaultAsync();
        }

    }
}
