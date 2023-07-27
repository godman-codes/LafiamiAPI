using LafiamiAPI.Datas.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LafiamiAPI.Interfaces.SystemInterfaces
{
    public interface IHospitalService : IRepositoryBase<HospitalModel, int>
    {
        IQueryable<HospitalInsurancePlanModel> GetHospitalInsurancePlanQueryable();
        Task<bool> IsHospitalInUse(int id);
        Task<bool> DoesHospitalsExist(List<int> ids);
        Task<bool> DoesHospitalExist(int id);
        Task<bool> IsHospitalNameInUse(string name, int stateId, int? cityId, int ignoreId = 0);
        void AddHospitalInsurancePlan(HospitalInsurancePlanModel hospitalInsurancePlan);
        void UpdateHospitalInsurancePlan(HospitalInsurancePlanModel hospitalInsurancePlan);
        void RemoveHospitalInsurancePlan(HospitalInsurancePlanModel hospitalInsurancePlan);
    }
}
