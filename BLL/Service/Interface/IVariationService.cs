using DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Service.Interface
{
    public interface IVariationService
    {
        Task<IList<Variation>> GetAllVariationsWithOptionsAsync();
    }
} 