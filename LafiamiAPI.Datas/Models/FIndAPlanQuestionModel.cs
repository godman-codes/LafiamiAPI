using System.Collections.Generic;

namespace LafiamiAPI.Datas.Models
{
    public partial class FIndAPlanQuestionModel : EntityBase<int>
    {
        public FIndAPlanQuestionModel() : base()
        {
            FindAPlanQuestionAnswers = new HashSet<FindAPlanQuestionAnswerModel>();
        }

        public string Question { get; set; }
        public string Note { get; set; }
        public bool IsActive { get; set; }
        public int OrderBy { get; set; }
        public bool HasDependency { get; set; }

        public virtual ICollection<FindAPlanQuestionAnswerModel> FindAPlanQuestionAnswers { get; set; }

    }
}
