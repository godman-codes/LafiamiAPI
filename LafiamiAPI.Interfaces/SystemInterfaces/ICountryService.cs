using LafiamiAPI.Datas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LafiamiAPI.Interfaces.SystemInterfaces
{
    public interface ICountryService : IRepositoryBase<CountryModel, int>
    {
        Task<int> GetCountryByState(int stateid);
        Task<int> GetCountryByCity(int cityid);
        Task<bool> IsCountryInUse(int id);
        Task<CountryModel> GetCountryByName(string name);
        Task<bool> GetCountryStatus(int id);
        Task<bool> DoesCountriesExist(List<int> ids);
        IQueryable<CountryModel> GetCountryQueryable(int page, int pageSize, Expression<Func<CountryModel, bool>> where);
        Task<bool> DoesCountryExist(int id);
        Task<bool> DoesCountryNameExist(string name, int ignoreId = 0);
        Task<CountryModel> GetCountry(int id);
    }
}
