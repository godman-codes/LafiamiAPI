using LafiamiAPI.Datas.Models;
using LafiamiAPI.Models.Requests;
using LafiamiAPI.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LafiamiAPI.Interfaces.SystemInterfaces
{
    public interface IVisitAuditService : IRepositoryBase<VisitModel, Guid>
    {
        IQueryable<UserSessionVisitModel> GetUserSessionVisitQueryable();
        IQueryable<PlanVisitModel> GetPlanVisitQueryable();

        Task<UserSessionVisitModel> GetTodayUserSessionVisitAsync(NewVisitRequest model, string userId);
        Task<PlanVisitModel> GetTodayPlanVisitAsync(long insurancePlanId, Guid sessionId);
        Task<int> GetUserCount(DateTime startDate, DateTime endDate, bool isUnique = false);
        Task<int> GetUserBounceCount(DateTime startDate, DateTime endDate, bool isUnique = false);
        Task<List<PlanCountResponse>> GetPlanUserCount(DateTime startDate, DateTime endDate, int topXPlans = 0);
        void AddCompareLog(CompareLogModel compareLog);
    }
}
