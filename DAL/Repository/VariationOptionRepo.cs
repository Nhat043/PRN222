using DAL.Datas;
using DAL.Models;
using DAL.Repository.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public class VariationOptionRepo : IVariationOptionRepo
    {
        private readonly DemoContext _demoContext;

        public VariationOptionRepo(DemoContext demoContext)
        {
            _demoContext = demoContext;
        }

        public async Task<IEnumerable<VariationOption>> GetAllVariationOptionsAsync()
        {
            return await _demoContext.VariationOptions
                .Include(vo => vo.Variation)
                .ToListAsync();
        }

        public async Task<VariationOption> GetVariationOptionByIdAsync(int id)
        {
            return await _demoContext.VariationOptions
                .Include(vo => vo.Variation)
                .FirstOrDefaultAsync(vo => vo.Id == id);
        }

        public async Task<bool> IsVariationOptionValueExistsAsync(string value, int variationId, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            var query = _demoContext.VariationOptions
                .Where(vo => vo.VariationId == variationId && 
                            vo.Value.Trim().ToLower() == value.Trim().ToLower());
            
            if (excludeId.HasValue)
                query = query.Where(vo => vo.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task<IEnumerable<VariationOption>> GetVariationOptionsByVariationIdAsync(int variationId)
        {
            return await _demoContext.VariationOptions
                .Where(vo => vo.VariationId == variationId)
                .Include(vo => vo.Variation)
                .ToListAsync();
        }

        public async Task AddVariationOptionAsync(VariationOption variationOption)
        {
            if (variationOption == null)
                throw new ArgumentNullException(nameof(variationOption));

            await _demoContext.VariationOptions.AddAsync(variationOption);
            await _demoContext.SaveChangesAsync();
        }

        public async Task UpdateVariationOptionAsync(VariationOption variationOption)
        {
            if (variationOption == null)
                throw new ArgumentNullException(nameof(variationOption));

            var existingVariationOption = await _demoContext.VariationOptions.FindAsync(variationOption.Id);
            if (existingVariationOption == null)
                throw new InvalidOperationException($"VariationOption with ID {variationOption.Id} not found.");

            // Update properties
            existingVariationOption.Value = variationOption.Value;
            existingVariationOption.VariationId = variationOption.VariationId;
            
            _demoContext.VariationOptions.Update(existingVariationOption);
            await _demoContext.SaveChangesAsync();
        }

        public async Task DeleteVariationOptionAsync(int id)
        {
            var variationOption = await _demoContext.VariationOptions.FindAsync(id);
            if (variationOption != null)
            {
                _demoContext.VariationOptions.Remove(variationOption);
                await _demoContext.SaveChangesAsync();
            }
        }
    }
} 