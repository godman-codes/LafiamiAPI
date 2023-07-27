using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LafiamiAPI.Models.Requests
{
    public class NewFIndAPlanRequest
    {
        [Required]
        public string Question { get; set; }
        public string Note { get; set; }
        public bool IsActive { get; set; }
        public int OrderBy { get; set; }
        public bool HasDependency { get; set; }
        [Required]
        public List<FindAPlanAnswerRequest> Answers { get; set; }
    }

    public class FindAPlanAnswerRequest
    {
        public int Id { get; set; }
        [Required]
        public string Answer { get; set; }
        public string Explanation { get; set; }
        public int OrderBy { get; set; }
        public int? DependentQuestionId { get; set; }
    }

    public class ExistingFIndAPlanRequest : NewFIndAPlanRequest
    {
        [Required(ErrorMessage = "Question Id is required")]
        public int Id { get; set; }
    }
}
