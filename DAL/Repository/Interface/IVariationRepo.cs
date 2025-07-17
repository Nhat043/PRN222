using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repository.Interface
{
    public interface IVariationRepo
    {
        Task<IList<Variation>> GetAllVariationsWithOptionsAsync();
    }
} 