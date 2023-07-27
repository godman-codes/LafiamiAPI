using LafiamiAPI.Datas.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LafiamiAPI.Interfaces.SystemInterfaces
{
    public interface ICityService : IRepositoryBase<CityModel, int>
    {
        Task<bool> IsCityInUse(int id);
        Task<CityModel> GetCityByName(string name);
        Task<bool> GetCityStatus(int id);
        Task<bool> DoesCitiesExist(List<int> ids);
        Task<bool> DoesCityExist(int id);
        Task<bool> DoesCityNameExist(string name, int ignoreId = 0);
        Task<CityModel> GetCity(int id);
        Task<CityModel> GetCityWithState(int id);
    }
}
