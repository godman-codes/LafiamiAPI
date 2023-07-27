using LafiamiAPI.Datas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LafiamiAPI.Interfaces.SystemInterfaces
{
    public interface IStateService : IRepositoryBase<StateModel, int>
    {
        Task<int> GetStateByCity(int cityid);
        Task<bool> IsStateInUse(int id);
        Task<StateModel> GetStateByName(string name);
        Task<bool> GetStateStatus(int id);
        Task<bool> DoesStatesExist(List<int> ids);
        IQueryable<StateModel> GetStateQueryable(int page, int pageSize, Expression<Func<StateModel, bool>> where);
        Task<bool> DoesStateExist(int id);
        Task<bool> DoesStateNameExist(string name, int ignoreId = 0);
        Task<StateModel> GetState(int id);
    }
}
