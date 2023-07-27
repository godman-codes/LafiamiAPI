using LafiamiAPI.Datas.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LafiamiAPI.Interfaces.BusinessInterfaces
{
    public interface ICategoryService : IRepositoryBase<CategoryModel, int>
    {
        Task<CategoryModel> GetCategory(int id);
        Task<bool> DoesCategoryExist(int id);
        Task<bool> IsNameInUse(string name, int IgnoreId = 0);
        Task<int> GetSubCategoryCount(int parentId);
        Task<bool> DoesCategoriesExist(List<int> ids);
    }
}
