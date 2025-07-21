using DAL.Datas;
using DAL.Models;
using DAL.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class VariationRepo : IVariationRepo
    {
        private readonly DemoContext _context;
        public VariationRepo(DemoContext context)
        {
            _context = context;
        }
        public async Task<IList<Variation>> GetAllVariationsWithOptionsAsync()
        {
            return await _context.Variations.Include(v => v.VariationOptions).ToListAsync();
        }
    }
} 