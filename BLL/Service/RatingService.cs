using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Service.Interface;
using DAL.Models;
using DAL.Repository.Interface;

namespace BLL.Service
{
    public class RatingService : IRatingService
    {
        private readonly IRatingRepo _ratingRepo;

        public RatingService(IRatingRepo ratingRepo)
        {
            _ratingRepo = ratingRepo;
        }

        public double GetAverageRating(int productId)
        {
            return _ratingRepo.GetAverageRating(productId);
        }

        public void RateProduct(int userId, int productId, int ratingValue)
        {
            var rating = new Rating
            {
                UserId = userId,
                ProductId = productId,
                RatingValue = ratingValue
            };

            _ratingRepo.InsertOrUpdateRating(rating);
        }
        public int GetReviewCount(int productId)
        {
            return _ratingRepo.GetReviewCount(productId);
        }
    }

}
