using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Picture { get; set; }

    public string? Description { get; set; }

    public int? CategoryId { get; set; }

    public int? StatusId { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<ProductItem> ProductItems { get; set; } = new List<ProductItem>();

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public virtual ProductStatus? Status { get; set; }
}
