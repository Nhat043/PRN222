using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;

namespace DAL.Repository.Interface
{
    public interface IComRepo
    {
        List<Comment> GetVisibleCommentsByProduct(int productId);
        void AddComment(Comment comment);
        void UpdateComment(Comment comment);
        void SoftDeleteComment(int commentId);
        Comment? GetCommentById(int id);

    }

}
