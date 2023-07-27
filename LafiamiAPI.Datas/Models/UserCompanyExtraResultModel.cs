using System;
using System.Collections.Generic;
using System.Text;

namespace LafiamiAPI.Datas.Models
{
    public partial class UserCompanyExtraResultModel
    {
        public string UserId { get; set; }
        public int CompanyExtraId { get; set; }
        public Guid OrderId { get; set; }

        public string ResultInString { get; set; }
        public string ResultInHTML { get; set; }
        public int? ResultInNumber { get; set; }
        public DateTime? ResultInDateTime { get; set; }
        public decimal? ResultInDecimal { get; set; }
        public bool? ResultInBool { get; set; }

        public virtual OrderModel Order { get; set; }
        public virtual UserViewModel User { get; set; }
        public virtual CompanyExtraModel CompanyExtra { get; set; }
    }
}
