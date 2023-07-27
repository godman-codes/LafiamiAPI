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
    public class BusinessUnitofWork : IBusinessUnitofWork
    {
        private readonly LafiamiContext _repoContext;
        private UserManager<UserViewModel> UserManager { get; set; }
        private RoleManager<ApplicationRoleModel> RoleManager { get; set; }
        public BusinessUnitofWork(LafiamiContext repoContext, UserManager<UserViewModel> userManager, RoleManager<ApplicationRoleModel> roleManager)
        {
            _repoContext = repoContext;
            UserManager = userManager;
            RoleManager = roleManager;
        }

        private IEmailService _Email;
        private IEmailTemplateService _EmailTemplate;
        private ICategoryService _Category;
        private IOrderService _Order;
        private ICartService _Cart;
        private IInsuranceService _Insurance;
        private IWalletService _Wallet;
        private IFindAPlanService _FindAPlanService;
        private IReviewService _ReviewService;
        private IUserService _user;

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

        public IReviewService ReviewService
        {
            get
            {
                if (_ReviewService == null)
                {
                    _ReviewService = new ReviewService(_repoContext);
                }

                return _ReviewService;
            }
        }

        public IFindAPlanService FindAPlanService
        {
            get
            {
                if (_FindAPlanService == null)
                {
                    _FindAPlanService = new FindAPlanService(_repoContext);
                }

                return _FindAPlanService;
            }
        }

        public IWalletService WalletService
        {
            get
            {
                if (_Wallet == null)
                {
                    _Wallet = new WalletService(_repoContext);
                }

                return _Wallet;
            }
        }

        public IInsuranceService InsuranceService
        {
            get
            {
                if (_Insurance == null)
                {
                    _Insurance = new InsuranceService(_repoContext);
                }

                return _Insurance;
            }
        }



        public ICartService CartService
        {
            get
            {
                if (_Cart == null)
                {
                    _Cart = new CartService(_repoContext);
                }

                return _Cart;
            }
        }

        public IOrderService OrderService
        {
            get
            {
                if (_Order == null)
                {
                    _Order = new OrderService(_repoContext);
                }

                return _Order;
            }
        }

        public ICategoryService CategoryService
        {
            get
            {
                if (_Category == null)
                {
                    _Category = new CategoryService(_repoContext);
                }

                return _Category;
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

        public IEmailService EmailService
        {
            get
            {
                if (_Email == null)
                {
                    _Email = new EmailService(_repoContext);
                }

                return _Email;
            }
        }

        public async Task SaveAsync()
        {
            await _repoContext.SaveChangesAsync();
        }
    }
}
