using System;
using System.Collections.Generic;
using System.Text;

namespace LafiamiAPI.Datas.Models
{
    public partial class UserSessionVisitModel : EntityBase<Guid>
    {
        public UserSessionVisitModel() : base()
        {
            Visits = new HashSet<VisitModel>();
        }

        public Guid SystemUserId { get; set; }
        public string UserId { get; set; }
        public virtual UserViewModel User { get; set; }
        public Guid SessionId { get; set; }
        public int SessionVisitCount { get; set; }
        public virtual ICollection<VisitModel> Visits { get; set; }
    }
}
