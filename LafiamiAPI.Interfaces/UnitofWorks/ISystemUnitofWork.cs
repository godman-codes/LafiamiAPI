using LafiamiAPI.Interfaces.BusinessInterfaces;
using LafiamiAPI.Interfaces.SystemInterfaces;
using System.Threading.Tasks;

namespace LafiamiAPI.Interfaces.UnitofWorks
{
    public interface ISystemUnitofWork
    {
        IPaymentService PaymentService { get; }
        ICityService CityService { get; }
        ICountryService CountryService { get; }
        IStateService StateService { get; }
        IEmailTemplateService EmailTemplateService { get; }
        IUserService UserService { get; }
        IManageCacheService ManageCacheService { get; }
        IHospitalService HospitalService { get; }
        IVisitAuditService VisitAuditService { get; }
        Task SaveAsync();
    }
}
