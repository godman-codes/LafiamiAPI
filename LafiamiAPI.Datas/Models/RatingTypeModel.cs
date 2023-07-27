using LafiamiAPI.Utilities.Enums;
using System;
using System.Collections.Generic;

namespace LafiamiAPI.Datas.Models
{
    public partial class RatingTypeModel : EntityBase<Guid>
    {
        public RatingTypeModel() : base()
        {
            TotalRatings = new HashSet<TotalRatingModel>();
            Ratings = new HashSet<RatingModel>();
        }

        public RatingFrequencyEnum RatingFrequency { get; set; }
        public bool IsActive { get; set; }
        public DateTime? EndDate { get; set; }
        public virtual ICollection<TotalRatingModel> TotalRatings { get; set; }
        public virtual ICollection<RatingModel> Ratings { get; set; }
    }
}
