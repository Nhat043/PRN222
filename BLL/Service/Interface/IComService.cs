using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;

namespace BLL.Service.Interface
{
    public interface IComService
    {
        List<Comment> GetProductComments(int productId);
        void CreateComment(Comment comment);
        void UpdateComment(Comment comment);
        void HideComment(int commentId);
        Comment GetById(int id);

        Task<List<Comment>> GetAllCommentsAsync();
        Task NotifyLoadAsync();
    }

}
