using System;
using System.Collections.Generic;

namespace DAL.Models;

public partial class Variation
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<VariationOption> VariationOptions { get; set; } = new List<VariationOption>();
}
