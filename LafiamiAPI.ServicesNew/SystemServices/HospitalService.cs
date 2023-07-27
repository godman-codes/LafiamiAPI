using LafiamiAPI.Datas;
using LafiamiAPI.Datas.Models;
using LafiamiAPI.Interfaces.SystemInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace LafiamiAPI.Services.SystemServices
{
    public class HospitalService : RepositoryBase<HospitalModel, int>, IHospitalService
    {
        public HospitalService(LafiamiContext repositoryContext)
        : base(repositoryContext)
        {
        }

        public IQueryable<HospitalInsurancePlanModel> GetHospitalInsurancePlanQueryable()
        {
            return DBContext.HospitalInsurancePlans.AsQueryable();
        }

        public async Task<bool> IsHospitalInUse(int id)
        {
            return await GetQueryable((r => (r.Id == id) && (r.HospitalInsurancePlans.Any()))).AnyAsync();
        }
        public async Task<bool> DoesHospitalExist(int id)
        {
            return await GetQueryable((r => r.Id == id)).AnyAsync();
        }
        public async Task<bool> DoesHospitalsExist(List<int> ids)
        {
            foreach (int id in ids)
            {
                if (!await DoesHospitalExist(id))
                {
                    return false;
                }
            }
            return true;
        }
        public async Task<bool> IsHospitalNameInUse(string name, int stateId, int? cityId, int ignoreId = 0)
        {
            IQueryable<HospitalModel> queryable = GetQueryable((r => r.Name.ToLower() == name.ToLower()));
            if (ignoreId > 0)
            {
                queryable = queryable.Where(r => r.Id != ignoreId);
            }
            if (stateId > 0)
            {
                queryable = queryable.Where(r => r.StateId == stateId);
            }
            if (cityId.HasValue && (cityId > 0))
            {
                queryable = queryable.Where(r => r.CityId == cityId.Value);
            }

            return await queryable.AnyAsync();
        }

        public void AddHospitalInsurancePlan(HospitalInsurancePlanModel hospitalInsurancePlan)
        {
            DBContext.HospitalInsurancePlans.Add(hospitalInsurancePlan);
        }
        public void UpdateHospitalInsurancePlan(HospitalInsurancePlanModel hospitalInsurancePlan)
        {
            DBContext.HospitalInsurancePlans.Update(hospitalInsurancePlan);
        }
        public void RemoveHospitalInsurancePlan(HospitalInsurancePlanModel hospitalInsurancePlan)
        {
            DBContext.HospitalInsurancePlans.Remove(hospitalInsurancePlan);
        }
    }
}
