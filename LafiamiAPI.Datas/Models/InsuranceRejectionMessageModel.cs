namespace LafiamiAPI.Datas.Models
{
    public partial class InsuranceRejectionMessageModel : EntityBase<long>
    {
        public InsuranceRejectionMessageModel() : base()
        {

        }
        public string Reason { get; set; }
        public long ProductId { get; set; }

        public virtual InsurancePlanModel InsurancePlan { get; set; }
    }
}
