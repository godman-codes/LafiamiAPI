using LafiamiAPI.Datas.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LafiamiAPI.Interfaces.BusinessInterfaces
{
    public interface IFindAPlanService : IRepositoryBase<FIndAPlanQuestionModel, int>
    {
        IQueryable<FindAPlanQuestionAnswerModel> GetFindAPlanQuestionAnswersQueryable();
        void DeleteFindAPlanQuestionAnswer(FindAPlanQuestionAnswerModel findAPlanQuestionAnswer);
        Task<bool> IsQuestionInUse(int questionid);
        Task<bool> DoesAnswerExist(int id);
        Task<bool> DoesAnswersExist(List<int> ids);
        void DeleteAnswerAsTag(InsurancePlanAnswerAsTagModel insurancePlanAnswerAsTag);

    }
}
