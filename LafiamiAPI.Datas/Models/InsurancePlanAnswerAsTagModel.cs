namespace LafiamiAPI.Datas.Models
{
    public partial class InsurancePlanAnswerAsTagModel
    {
        public long InsurancePlanId { get; set; }
        public virtual InsurancePlanModel InsurancePlan { get; set; }
        public int FindAPlanQuestionAnswerId { get; set; }
        public virtual FindAPlanQuestionAnswerModel FindAPlanQuestionAnswer { get; set; }
    }
}
