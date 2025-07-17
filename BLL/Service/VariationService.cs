using BLL.Service.Interface;
using DAL.Models;
using DAL.Repository.Interface;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Service
{
    public class VariationService : IVariationService
    {
        private readonly IVariationRepo _variationRepo;
        public VariationService(IVariationRepo variationRepo)
        {
            _variationRepo = variationRepo;
        }
        public async Task<IList<Variation>> GetAllVariationsWithOptionsAsync()
        {
            return await _variationRepo.GetAllVariationsWithOptionsAsync();
        }
    }
} 