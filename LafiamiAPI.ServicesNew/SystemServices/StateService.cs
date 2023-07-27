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
    public class StateService : RepositoryBase<StateModel, int>, IStateService
    {
        public StateService(LafiamiContext repositoryContext)
        : base(repositoryContext)
        {
        }

        public IQueryable<StateModel> GetStateQueryable(int page, int pageSize, Expression<Func<StateModel, bool>> where)
        {
            IQueryable<StateModel> queryable = GetQueryable(where);
            if (pageSize > 0)
            {
                queryable = queryable.OrderByDescending(r => r.CreatedDate)
                    .Skip(page * pageSize)
                    .Take(pageSize);
            }

            return queryable;
        }
        public async Task<bool> DoesStateExist(int id)
        {
            return await GetQueryable((r => r.Id == id)).AnyAsync();
        }

        public async Task<bool> DoesStatesExist(List<int> ids)
        {
            foreach (int id in ids)
            {
                if (!await DoesStateExist(id))
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> DoesStateNameExist(string name, int ignoreId = 0)
        {
            IQueryable<StateModel> queryable = GetQueryable((r => r.Name.ToLower() == name.ToLower()));
            if (ignoreId > 0)
            {
                queryable = queryable.Where(r => r.Id != ignoreId);
            }
            return await queryable.AnyAsync();
        }

        public async Task<StateModel> GetState(int id)
        {
            return await GetQueryable((r => r.Id == id)).FirstOrDefaultAsync();
        }

        public async Task<int> GetStateByCity(int cityid)
        {
            return await GetQueryable((r => r.Cities.Any(t => t.Id == cityid)))
                .Select(r => r.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> GetStateStatus(int id)
        {
            return await GetQueryable(r => (r.Id == id)).Select(r => r.Enable).SingleOrDefaultAsync();
        }
        public async Task<StateModel> GetStateByName(string name)
        {
            IQueryable<StateModel> queryable = GetQueryable((r => EF.Functions.Like(r.Name, name)));
            return await queryable.SingleOrDefaultAsync();
        }

        public async Task<bool> IsStateInUse(int id)
        {
            //if websiteId is null, more than one website is using it. 
            return await GetQueryable((r => (r.Id == id) && (r.Users.Any() || r.Orders.Any(t => !t.IsDeleted)))).AnyAsync();
        }

    }
}
