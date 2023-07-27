using LafiamiAPI.Datas;
using LafiamiAPI.Datas.Models;
using LafiamiAPI.Interfaces.BusinessInterfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LafiamiAPI.Services.BusinessServices
{
    public class FindAPlanService : RepositoryBase<FIndAPlanQuestionModel, int>, IFindAPlanService
    {
        public FindAPlanService(LafiamiContext repositoryContext)
        : base(repositoryContext)
        {
        }

        public IQueryable<FindAPlanQuestionAnswerModel> GetFindAPlanQuestionAnswersQueryable()
        {
            return DBContext.FindAPlanQuestionAnswers.Where(r => !r.IsDeleted).AsQueryable();
        }

        public void DeleteFindAPlanQuestionAnswer(FindAPlanQuestionAnswerModel findAPlanQuestionAnswer)
        {
            DBContext.FindAPlanQuestionAnswers.Remove(findAPlanQuestionAnswer);
        }


        public void DeleteAnswerAsTag(InsurancePlanAnswerAsTagModel insurancePlanAnswerAsTag)
        {
            DBContext.InsurancePlanAnswerAsTags.Remove(insurancePlanAnswerAsTag);
        }

        public async Task<bool> IsQuestionInUse(int questionid)
        {
            //if websiteId is null, more than one website is using it. 
            return await DBContext.InsurancePlanAnswerAsTags.Where(r => r.FindAPlanQuestionAnswer.FIndAPlanQuestionId == questionid).AnyAsync();
        }

        public async Task<bool> DoesAnswerExist(int id)
        {
            return await GetFindAPlanQuestionAnswersQueryable().Where((r => r.Id == id)).AnyAsync();
        }
        public async Task<bool> DoesAnswersExist(List<int> ids)
        {
            foreach (int id in ids)
            {
                if (!await DoesAnswerExist(id))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
