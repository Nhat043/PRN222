using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Service.Interface
{
    public interface IRatingService
    {
        double GetAverageRating(int productId);
        void RateProduct(int userId, int productId, int ratingValue);
        int GetReviewCount(int productId);
    }

}
