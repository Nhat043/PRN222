using BLL.Service.Interface;
using DAL.Models;
using DAL.Repository.Interface;
using System.Collections.Generic;

namespace BLL.Service
{
    public class CommentStatusService : ICommentStatusService
    {
        private readonly ICommentStatusRepo _repo;

        public CommentStatusService(ICommentStatusRepo repo)
        {
            _repo = repo;
        }

        public List<CommentStatus> GetAll()
        {
            return _repo.GetAll();
        }

        public CommentStatus? GetById(int id)
        {
            return _repo.GetById(id);
        }

        public int? GetIdByName(string name)
        {
            return _repo.GetIdByName(name);
        }
    }
}

