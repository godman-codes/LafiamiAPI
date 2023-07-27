using LafiamiAPI.Interfaces.BusinessInterfaces;
using LafiamiAPI.Interfaces.SystemInterfaces;
using System.Threading.Tasks;

namespace LafiamiAPI.Interfaces.UnitofWorks
{
    public interface IBusinessUnitofWork
    {
        IEmailService EmailService { get; }
        IEmailTemplateService EmailTemplateService { get; }
        ICategoryService CategoryService { get; }
        IOrderService OrderService { get; }
        ICartService CartService { get; }
        IInsuranceService InsuranceService { get; }
        IWalletService WalletService { get; }
        IFindAPlanService FindAPlanService { get; }
        IReviewService ReviewService { get; }
        IUserService UserService { get; }
        Task SaveAsync();
    }
}
