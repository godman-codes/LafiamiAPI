using System;
using System.Collections.Generic;
using System.Text;

namespace LafiamiAPI.Datas.Models
{
    public partial class InsuranceAuditCategoryModel
    {
        public int CategoryId { get; set; }
        public Guid InsuranceAuditId { get; set; }

        public virtual CategoryModel Category { get; set; }
        public virtual InsuranceAuditModel InsuranceAudit { get; set; }
    }
}
