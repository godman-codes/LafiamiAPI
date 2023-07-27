using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace LafiamiAPI.Datas.Models
{
    public partial class ApplicationRoleModel : IdentityRole
    {
        public ApplicationRoleModel() : base()
        {
            UserRoles = new HashSet<ApplicationUserRoleModel>();
        }
        public virtual ICollection<ApplicationUserRoleModel> UserRoles { get; set; }
    }
}
