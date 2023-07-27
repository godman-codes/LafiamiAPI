using LafiamiAPI.Datas;
using LafiamiAPI.Datas.Models;
using LafiamiAPI.Interfaces.BusinessInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LafiamiAPI.Services.BusinessServices
{
    public class CategoryService : RepositoryBase<CategoryModel, int>, ICategoryService
    {
        public CategoryService(LafiamiContext repositoryContext)
        : base(repositoryContext)
        {
        }

        public async Task<CategoryModel> GetCategory(int id)
        {
            return await GetQueryable((r => r.Id == id)).FirstOrDefaultAsync();
        }

        public async Task<int> GetSubCategoryCount(int parentId)
        {
            return await GetQueryable((r => r.ParentId == parentId)).CountAsync();
        }

        public async Task<bool> DoesCategoryExist(int id)
        {
            return await GetQueryable((r => r.Id == id)).AnyAsync();
        }

        public async Task<bool> DoesCategoriesExist(List<int> ids)
        {
            foreach (int id in ids)
            {
                if (!await DoesCategoryExist(id))
                {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> IsNameInUse(string name, int IgnoreId = 0)
        {
            name = name.Trim();
            IQueryable<CategoryModel> queryable = GetQueryable((r => EF.Functions.Like(r.Name, name)));
            if (IgnoreId > 0)
            {
                queryable = queryable.Where(r => r.Id != IgnoreId);
            }
            return await queryable.AnyAsync();
        }


    }
}
