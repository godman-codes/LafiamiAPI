using System;
using System.Collections.Generic;
using System.Text;

namespace LafiamiAPI.Datas.Models
{
    public partial class InsuranceAuditQuestionAnswerModel
    {
        public int FindAPlanQuestionAnswerId { get; set; }
        public Guid InsuranceAuditId { get; set; }

        public virtual FindAPlanQuestionAnswerModel FindAPlanQuestionAnswer { get; set; }
        public virtual InsuranceAuditModel InsuranceAudit { get; set; }
    }
}
