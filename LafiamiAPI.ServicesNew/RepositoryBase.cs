using LafiamiAPI.Datas;
using LafiamiAPI.Datas.Interfaces;
using LafiamiAPI.Interfaces;
using LafiamiAPI.Utilities.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LafiamiAPI.Services
{
    public abstract class RepositoryBase<T, R> : IRepositoryBase<T, R> where T : class, IEntityBase<R>, new()
    {
        protected LafiamiContext DBContext { get; set; }

        public RepositoryBase(LafiamiContext repositoryContext)
        {
            DBContext = repositoryContext;
        }

        public IQueryable<T> GetQueryable(Expression<Func<T, bool>> condition)
        {
            Expression<Func<T, bool>> whereClause1 = (p => !p.IsDeleted);
            Expression<Func<T, bool>> where = QueryCombinator.MergeWithAnd<T>(condition, whereClause1);

            return DBContext.Set<T>().Where(where);
        }

        public IQueryable<T> GetAllQueryable()
        {
            return DBContext.Set<T>();
        }

        public void Add(T entity)
        {
            DBContext.Set<T>().Add(entity);
        }

        public void AddRange(List<T> entities)
        {
            DBContext.Set<T>().AddRange(entities);
        }

        public void Update(T entity)
        {
            DBContext.Set<T>().Update(entity);
        }

        public void Delete(T entity)
        {
            DBContext.Set<T>().Remove(entity);
        }

        public async Task SaveAsync()
        {
            await DBContext.SaveChangesAsync();
        }

    }
}
