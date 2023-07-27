using System.Collections.Generic;

namespace LafiamiAPI.Models.Responses
{
    public class LiteFindAPlanResponse
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public bool IsActive { get; set; }
        public int AnswerCount { get; set; }
    }

    public class FindAPlanResponse
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public string Note { get; set; }
        public bool IsActive { get; set; }
        public int OrderBy { get; set; }
        public bool HasDependency { get; set; }
        public List<FindAPlanAnswerResponse> Answers { get; set; }
    }

    public class FindAPlanAnswerResponse
    {
        public int Id { get; set; }
        public string Answer { get; set; }
        public string Explanation { get; set; }
        public int OrderBy { get; set; }
        public int? DependentQuestionId { get; set; }
    }

    public class ViewFindAPlanResponse
    {
        public int Id { get; set; }
        public string Question { get; set; }
        public int OrderBy { get; set; }
        public List<ViewFindAPlanAnswerResponse> Answers { get; set; }
    }

    public class ViewFindAPlanAnswerResponse
    {
        public int Id { get; set; }
        public string Answer { get; set; }
        public int OrderBy { get; set; }
    }
}
