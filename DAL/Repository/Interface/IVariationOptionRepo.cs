using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository.Interface
{
    public interface IVariationOptionRepo
    {
        Task<IEnumerable<VariationOption>> GetAllVariationOptionsAsync();
        Task<VariationOption> GetVariationOptionByIdAsync(int id);
        Task<bool> IsVariationOptionValueExistsAsync(string value, int variationId, int? excludeId = null);
        Task<IEnumerable<VariationOption>> GetVariationOptionsByVariationIdAsync(int variationId);

        Task AddVariationOptionAsync(VariationOption variationOption);
        Task UpdateVariationOptionAsync(VariationOption variationOption);
        Task DeleteVariationOptionAsync(int id);
    }
} 