using BLL.Service.Interface;
using DAL.Models;
using DAL.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Service
{
    public class VariationOptionService : IVariationOptionService
    {
        private readonly IVariationOptionRepo _variationOptionRepo;

        public VariationOptionService(IVariationOptionRepo variationOptionRepo)
        {
            _variationOptionRepo = variationOptionRepo;
        }

        private void ValidateVariationOption(VariationOption variationOption)
        {
            if (variationOption == null)
                throw new ArgumentNullException(nameof(variationOption));
        }

        public async Task<IEnumerable<VariationOption>> GetAllVariationOptionsAsync()
        {
            return await _variationOptionRepo.GetAllVariationOptionsAsync();
        }

        public async Task<VariationOption> GetVariationOptionByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("VariationOption ID must be greater than zero.", nameof(id));

            var variationOption = await _variationOptionRepo.GetVariationOptionByIdAsync(id);
            if (variationOption == null)
                throw new InvalidOperationException($"VariationOption with ID {id} not found.");

            return variationOption;
        }

        public async Task<bool> IsVariationOptionValueExistsAsync(string value, int variationId, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            return await _variationOptionRepo.IsVariationOptionValueExistsAsync(value, variationId, excludeId);
        }

        public async Task<IEnumerable<VariationOption>> GetVariationOptionsByVariationIdAsync(int variationId)
        {
            return await _variationOptionRepo.GetVariationOptionsByVariationIdAsync(variationId);
        }

        public async Task AddVariationOptionAsync(VariationOption variationOption)
        {
            // Business logic validation
            ValidateVariationOption(variationOption);

            // Check for duplicate variation option value within the same variation
            if (await IsVariationOptionValueExistsAsync(variationOption.Value, variationOption.VariationId ?? 0))
            {
                throw new InvalidOperationException($"A variation option with the value '{variationOption.Value}' already exists for this variation.");
            }

            await _variationOptionRepo.AddVariationOptionAsync(variationOption);
        }

        public async Task UpdateVariationOptionAsync(VariationOption variationOption)
        {
            // Business logic validation
            ValidateVariationOption(variationOption);

            if (variationOption.Id <= 0)
                throw new ArgumentException("VariationOption ID must be greater than zero.", nameof(variationOption.Id));

            // Check for duplicate variation option value within the same variation (excluding current option)
            if (await IsVariationOptionValueExistsAsync(variationOption.Value, variationOption.VariationId ?? 0, variationOption.Id))
            {
                throw new InvalidOperationException($"A variation option with the value '{variationOption.Value}' already exists for this variation.");
            }

            await _variationOptionRepo.UpdateVariationOptionAsync(variationOption);
        }

        public async Task DeleteVariationOptionAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("VariationOption ID must be greater than zero.", nameof(id));

            // Additional business logic can be added here
            // For example: Check if variation option is referenced in product items before deletion

            await _variationOptionRepo.DeleteVariationOptionAsync(id);
        }
    }
} 