using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Datas;
using DAL.Models;
using DAL.Repository.Interface;

namespace DAL.Repository
{
    public class RatingRepo : IRatingRepo
    {
        private readonly DemoContext _context;

        public RatingRepo(DemoContext context)
        {
            _context = context;
        }

        public double GetAverageRating(int productId)
        {
            var ratings = _context.Ratings.Where(r => r.ProductId == productId && r.RatingValue.HasValue);
            return ratings.Any() ? ratings.Average(r => r.RatingValue.Value) : 5.0;
        }

        public void InsertOrUpdateRating(Rating rating)
        {
            var existing = _context.Ratings
                .FirstOrDefault(r => r.UserId == rating.UserId && r.ProductId == rating.ProductId);

            if (existing != null)
            {
                existing.RatingValue = rating.RatingValue;
                existing.CreatedAt = DateTime.Now;
            }
            else
            {
                rating.CreatedAt = DateTime.Now;
                _context.Ratings.Add(rating);
            }

            _context.SaveChanges();
        }
    }

}
