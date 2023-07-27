using System;
using System.Collections.Generic;

namespace LafiamiAPI.Datas.Models
{
    public partial class TotalRatingModel : EntityBase<Guid>
    {
        public TotalRatingModel() : base()
        {
            Ratings = new HashSet<RatingModel>();
        }

        public Guid RatingTypeId { get; set; }
        public virtual RatingTypeModel RatingType { get; set; }
        public byte TotalStarRating { get; set; }

        public long InsurancePlanId { get; set; }
        public virtual InsurancePlanModel InsurancePlan { get; set; }

        public virtual ICollection<RatingModel> Ratings { get; set; }
    }
}
