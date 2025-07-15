using BLL.Service.Interface;
using DAL.Datas;
using DAL.Models;
using System.Collections.Generic;
using System.Linq;

namespace BLL.Service
{
    public class CommentStatusService : ICommentStatusService
    {
        private readonly DemoContext _context;

        public CommentStatusService(DemoContext context)
        {
            _context = context;
        }

        public List<CommentStatus> GetAll()
        {
            return _context.CommentStatuses.ToList();
        }

        public CommentStatus? GetById(int id)
        {
            return _context.CommentStatuses.FirstOrDefault(x => x.Id == id);
        }

        public int? GetIdByName(string name)
        {
            return _context.CommentStatuses.FirstOrDefault(s => s.Name == name)?.Id;
        }
    }
}
