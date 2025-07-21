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
    public class ComService : IComService
    {
        private readonly IComRepo _repo;

        public ComService(IComRepo repo)
        {
            _repo = repo;
        }

        public List<Comment> GetProductComments(int productId) => _repo.GetVisibleCommentsByProduct(productId);

        public void CreateComment(Comment comment) => _repo.AddComment(comment);

        public void UpdateComment(Comment comment) => _repo.UpdateComment(comment);

        public void HideComment(int commentId) => _repo.SoftDeleteComment(commentId);

        public Comment GetById(int id) => _repo.GetCommentById(id);

        public async Task<List<Comment>> GetAllCommentsAsync()
        {
            return await _repo.GetAllCommentsAsync();
        }

    }

}
