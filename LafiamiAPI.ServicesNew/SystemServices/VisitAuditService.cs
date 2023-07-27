using LafiamiAPI.Datas;
using LafiamiAPI.Datas.Models;
using LafiamiAPI.Interfaces.SystemInterfaces;
using LafiamiAPI.Models.Requests;
using LafiamiAPI.Models.Responses;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LafiamiAPI.Services.SystemServices
{
    public class VisitAuditService : RepositoryBase<VisitModel, Guid>, IVisitAuditService
    {
        public VisitAuditService(LafiamiContext repositoryContext)
        : base(repositoryContext)
        {
        }

        public IQueryable<UserSessionVisitModel> GetUserSessionVisitQueryable()
        {
            return DBContext.UserSessionVisits.Where(r => !r.IsDeleted).AsQueryable();
        }

        public IQueryable<PlanVisitModel> GetPlanVisitQueryable()
        {
            return DBContext.PlanVisits.Where(r => !r.IsDeleted).AsQueryable();
        }

        public async Task<UserSessionVisitModel> GetTodayUserSessionVisitAsync(NewVisitRequest model, string userId)
        {
            IQueryable<UserSessionVisitModel> queryable = GetUserSessionVisitQueryable().Where(r => (r.SessionId == model.SessionId) && (r.SystemUserId == model.SystemUserId));
            if (!string.IsNullOrEmpty(userId))
            {
                queryable = queryable.Where(r => r.UserId == userId);
            }

            DateTime today = DateTime.Today;
            queryable = queryable.Where(r => r.CreatedDate.Date == today);

            return await queryable.SingleOrDefaultAsync();
        }

        public async Task<PlanVisitModel> GetTodayPlanVisitAsync(long insurancePlanId, Guid sessionId)
        {
            IQueryable<PlanVisitModel> queryable = GetPlanVisitQueryable().Where(r => (r.SessionId == sessionId) && (r.InsurancePlanId == insurancePlanId));
            DateTime today = DateTime.Today;
            queryable = queryable.Where(r => r.CreatedDate.Date == today);

            return await queryable.SingleOrDefaultAsync();
        }

        public async Task<int> GetUserCount(DateTime startDate, DateTime endDate, bool isUnique = false)
        {
            IQueryable<UserSessionVisitModel> queryable = GetUserSessionVisitQueryable().Where(r => (startDate <= r.CreatedDate) && (r.CreatedDate <= endDate));
            return await (isUnique ? queryable.GroupBy(r => r.SystemUserId).CountAsync() : queryable.CountAsync());
        }

        public async Task<int> GetUserBounceCount(DateTime startDate, DateTime endDate, bool isUnique = false)
        {
            IQueryable<UserSessionVisitModel> queryable = GetUserSessionVisitQueryable().Where(r => (startDate <= r.CreatedDate) && (r.CreatedDate <= endDate));
            queryable = queryable.Where(r => r.SessionVisitCount == 1);

            return await (isUnique ? queryable.GroupBy(r => r.SystemUserId).CountAsync() : queryable.CountAsync());
        }

        public async Task<List<PlanCountResponse>> GetPlanUserCount(DateTime startDate, DateTime endDate, int topXPlans = 0)
        {
            IQueryable<PlanVisitModel> queryable = GetPlanVisitQueryable().Where(r => (startDate <= r.CreatedDate) && (r.CreatedDate <= endDate));
            if (topXPlans > 0)
            {
                queryable = queryable.OrderBy(r => r.CreatedDate).Take(topXPlans);
            }

            return await queryable.Select(r => new PlanCountResponse()
            {
                InsurancePlanName = r.InsurancePlan.Name,
                VisitCount = r.VisitCount
            }).ToListAsync();
        }

        public void AddCompareLog(CompareLogModel compareLog)
        {
            DBContext.CompareLogs.Add(compareLog);
        }

    }
}
