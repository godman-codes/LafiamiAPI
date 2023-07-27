using System;

namespace LafiamiAPI.Datas.Models
{
    public partial class RatingModel : EntityBase<Guid>
    {
        public RatingModel() : base()
        {
        }

        public byte StarRating { get; set; }
        public string UserId { get; set; }
        public virtual UserViewModel User { get; set; }
        public Guid RatingTypeId { get; set; }
        public virtual RatingTypeModel RatingType { get; set; }



        public long InsurancePlanId { get; set; }
        public virtual InsurancePlanModel InsurancePlan { get; set; }

        public virtual TotalRatingModel TotalRating { get; set; }
    }
}
