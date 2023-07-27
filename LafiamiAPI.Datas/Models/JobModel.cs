using System;
using System.Collections.Generic;
using System.Text;

namespace LafiamiAPI.Datas.Models
{
    public partial class JobModel : EntityBase<Guid>
    {
        public JobModel() : base()
        {

        }

        public string CompanyName { get; set; }
        public string CompanyAddress { get; set; }
        public string JobTitle { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrentJob { get; set; }
        public string UserId { get; set; }
        public virtual UserViewModel User { get; set; }
        public Guid? OrderId { get; set; }
        public virtual OrderModel Order { get; set; }
    }
}
