using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Rating
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? ProductId { get; set; }

    public int? RatingValue { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Product? Product { get; set; }

    public virtual Account? User { get; set; }
}
