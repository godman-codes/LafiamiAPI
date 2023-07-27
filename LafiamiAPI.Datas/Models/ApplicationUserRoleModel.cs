using Microsoft.AspNetCore.Identity;

namespace LafiamiAPI.Datas.Models
{
    public partial class ApplicationUserRoleModel : IdentityUserRole<string>
    {
        public ApplicationUserRoleModel() : base()
        {

        }
        public virtual UserViewModel User { get; set; }
        public virtual ApplicationRoleModel Role { get; set; }
    }
}
