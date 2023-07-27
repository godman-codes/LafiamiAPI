using LafiamiAPI.Datas.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LafiamiAPI.Interfaces
{
    public interface IRepositoryBase<T, R> where T : class, IEntityBase<R>, new()
    {
        /// <summary>
        /// This return IQueryable records that are not marks as deleted
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        IQueryable<T> GetQueryable(Expression<Func<T, bool>> condition);
        /// <summary>
        /// This return all IQueryable records including those marks as deleted
        /// </summary>
        /// <returns></returns>
        IQueryable<T> GetAllQueryable();
        void Add(T entity);
        void AddRange(List<T> entities);
        void Update(T entity);
        void Delete(T entity);
        Task SaveAsync();
    }
}
