using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Comment
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? ProductId { get; set; }

    public string Content { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public int? StatusId { get; set; }

    public int? ParentId { get; set; }

    public virtual ICollection<Comment> InverseParent { get; set; } = new List<Comment>();

    public virtual Comment? Parent { get; set; }

    public virtual Product? Product { get; set; }

    public virtual CommentStatus? Status { get; set; }

    public virtual Account? User { get; set; }
   

}
