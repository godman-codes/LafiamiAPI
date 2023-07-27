namespace LafiamiAPI.Datas.Models
{
    public partial class InsuranceCategory
    {
        public int CategoryId { get; set; }
        public long InsurancePlanId { get; set; }
        public bool IsCovered { get; set; }
        public string CoveredNote { get; set; }

        public virtual CategoryModel Category { get; set; }
        public virtual InsurancePlanModel InsurancePlan { get; set; }
    }
}
