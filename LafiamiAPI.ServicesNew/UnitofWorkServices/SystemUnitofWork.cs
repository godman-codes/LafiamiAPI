using LafiamiAPI.Datas;
using LafiamiAPI.Datas.Models;
using LafiamiAPI.Interfaces.BusinessInterfaces;
using LafiamiAPI.Interfaces.SystemInterfaces;
using LafiamiAPI.Interfaces.UnitofWorks;
using LafiamiAPI.Services.BusinessServices;
using LafiamiAPI.Services.SystemServices;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace LafiamiAPI.Services.UnitofWorkServices
{
    public class SystemUnitofWork : ISystemUnitofWork
    {
        private readonly LafiamiContext _repoContext;
        private UserManager<UserViewModel> UserManager { get; set; }
        private RoleManager<ApplicationRoleModel> RoleManager { get; set; }
        public SystemUnitofWork(LafiamiContext repoContext, UserManager<UserViewModel> userManager, RoleManager<ApplicationRoleModel> roleManager)
        {
            _repoContext = repoContext;
            UserManager = userManager;
            RoleManager = roleManager;
        }
        private IPaymentService _Payment;
        private ICityService _City;
        private ICountryService _Country;
        private IStateService _State;
        private IEmailTemplateService _EmailTemplate;
        private IUserService _user;
        private IManageCacheService _manageService;
        private IHospitalService _HospitalService;
        private IVisitAuditService _VisitAuditService;

        public IVisitAuditService VisitAuditService
        {
            get
            {
                if (_VisitAuditService == null)
                {
                    _VisitAuditService = new VisitAuditService(_repoContext);
                }

                return _VisitAuditService;
            }
        }

        public IHospitalService HospitalService
        {
            get
            {
                if (_HospitalService == null)
                {
                    _HospitalService = new HospitalService(_repoContext);
                }

                return _HospitalService;
            }
        }

        public IManageCacheService ManageCacheService
        {
            get
            {
                if (_manageService == null)
                {
                    _manageService = new ManageCacheService(_repoContext);
                }

                return _manageService;
            }
        }


        public IUserService UserService
        {
            get
            {
                if (_user == null)
                {
                    _user = new UserService(_repoContext, UserManager, RoleManager);
                }

                return _user;
            }
        }

        public IEmailTemplateService EmailTemplateService
        {
            get
            {
                if (_EmailTemplate == null)
                {
                    _EmailTemplate = new EmailTemplateService(_repoContext);
                }

                return _EmailTemplate;
            }
        }

        public IPaymentService PaymentService
        {
            get
            {
                if (_Payment == null)
                {
                    _Payment = new PaymentService(_repoContext);
                }

                return _Payment;
            }
        }

        public ICityService CityService
        {
            get
            {
                if (_City == null)
                {
                    _City = new CityService(_repoContext);
                }

                return _City;
            }
        }
        public ICountryService CountryService
        {
            get
            {
                if (_Country == null)
                {
                    _Country = new CountryService(_repoContext);
                }

                return _Country;
            }
        }

        public IStateService StateService
        {
            get
            {
                if (_State == null)
                {
                    _State = new StateService(_repoContext);
                }

                return _State;
            }
        }

        public async Task SaveAsync()
        {
            await _repoContext.SaveChangesAsync();
        }
    }
}
