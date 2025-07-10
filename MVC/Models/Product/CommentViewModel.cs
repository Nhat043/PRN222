using DAL.Models;

namespace MVC.Models.Product
{
    public class CommentViewModel
    {
        public List<Comment> Comments { get; set; } = new();
        public int? CurrentUserId { get; set; }
        public string? NewCommentContent { get; set; }
    }
}
