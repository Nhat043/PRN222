using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class CommentStatus
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
