using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Datas;
using DAL.Models;
using DAL.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository
{
    public class ComRepo : IComRepo
    {
        private readonly DemoContext _context;

        public ComRepo(DemoContext context)
        {
            _context = context;
        }

        public List<Comment> GetVisibleCommentsByProduct(int productId)
        {
            return _context.Comments
                .Where(c => c.ProductId == productId && c.StatusId == 1)
                 .Include(c => c.User)
                .OrderByDescending(c => c.CreatedAt)
                .ToList();
        }

        public void AddComment(Comment comment)
        {
            comment.CreatedAt = DateTime.Now;
            comment.StatusId = 1; // visible
            _context.Comments.Add(comment);
            _context.SaveChanges();
        }

        public void UpdateComment(Comment comment)
        {
            _context.Comments.Update(comment);
            _context.SaveChanges();
        }

        public void SoftDeleteComment(int commentId)
        {
            var comment = _context.Comments.FirstOrDefault(c => c.Id == commentId);
            if (comment != null)
            {
                comment.StatusId = 2; // hidden
                _context.SaveChanges();
            }
        }

        public Comment? GetCommentById(int id)
        {
            return _context.Comments.Include(c => c.User).FirstOrDefault(c => c.Id == id);
        }

    }

}
