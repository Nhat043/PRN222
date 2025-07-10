using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;

namespace DAL.Repository.Interface
{
    public interface IRatingRepo
    {
        double GetAverageRating(int productId);
        void InsertOrUpdateRating(Rating rating);
    }

}
