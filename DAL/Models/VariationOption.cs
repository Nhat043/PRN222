using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class VariationOption
{
    public int Id { get; set; }

    public int? VariationId { get; set; }

    public string Value { get; set; } = null!;

    public virtual Variation? Variation { get; set; }

    public virtual ICollection<ProductItem> ProductItems { get; set; } = new List<ProductItem>();
}
