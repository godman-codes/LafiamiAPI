using LafiamiAPI.Datas;
using LafiamiAPI.Datas.Models;
using LafiamiAPI.Interfaces.BusinessInterfaces;
using System;

namespace LafiamiAPI.Services.BusinessServices
{
    public class ReviewService : RepositoryBase<ReviewModel, Guid>, IReviewService
    {
        public ReviewService(LafiamiContext repositoryContext)
        : base(repositoryContext)
        {
        }

    }
}
