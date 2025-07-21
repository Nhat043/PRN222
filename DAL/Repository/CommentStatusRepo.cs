using DAL.Datas;
using DAL.Models;
using DAL.Repository.Interface;
using System.Collections.Generic;
using System.Linq;

namespace DAL.Repository
{
    public class CommentStatusRepo : ICommentStatusRepo
    {
        private readonly DemoContext _context;

        public CommentStatusRepo(DemoContext context)
        {
            _context = context;
        }

        public List<CommentStatus> GetAll()
        {
            return _context.CommentStatuses.ToList();
        }

        public CommentStatus? GetById(int id)
        {
            return _context.CommentStatuses.FirstOrDefault(cs => cs.Id == id);
        }

        public int? GetIdByName(string name)
        {
            return _context.CommentStatuses.FirstOrDefault(cs => cs.Name == name)?.Id;
        }
    }
}
