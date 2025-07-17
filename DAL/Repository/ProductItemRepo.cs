using DAL.Datas;
using DAL.Models;
using DAL.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repository
{
    public class ProductItemRepo : IProductItemRepo
    {
        private readonly DemoContext _context;

        public ProductItemRepo(DemoContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductItem>> GetAllProductItemsAsync()
        {
            return await _context.ProductItems
                .Include(pi => pi.VariationOptions)
                    .ThenInclude(vo => vo.Variation)
                .Include(pi => pi.Product)
                .Include(pi => pi.Status)
                .ToListAsync();
        }

        public async Task<ProductItem?> GetProductItemByIdAsync(int id)
        {
            return await _context.ProductItems
                .Include(pi => pi.VariationOptions)
                    .ThenInclude(vo => vo.Variation)
                .Include(pi => pi.Product)
                .Include(pi => pi.Status)
                .FirstOrDefaultAsync(pi => pi.Id == id);
        }

        public async Task AddProductItemAsync(ProductItem item)
        {
            await _context.ProductItems.AddAsync(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductItemAsync(ProductItem item)
        {
            _context.ProductItems.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductItemAsync(int id)
        {
            var item = await _context.ProductItems.FindAsync(id);
            if (item != null)
            {
                _context.ProductItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IList<ProductItem>> GetProductItemsByProductIdAsync(int productId)
        {
            return await _context.ProductItems
                .Include(pi => pi.Status)
                .Include(pi => pi.VariationOptions)
                .Where(pi => pi.ProductId == productId)
                .ToListAsync();
        }

        public async Task SetVariationOptionsAsync(int productItemId, List<int> variationOptionIds)
        {
            var productItem = await _context.ProductItems
                .Include(pi => pi.VariationOptions)
                .FirstOrDefaultAsync(pi => pi.Id == productItemId);
            if (productItem == null) return;
            productItem.VariationOptions.Clear();
            if (variationOptionIds != null && variationOptionIds.Count > 0)
            {
                var options = await _context.VariationOptions.Where(vo => variationOptionIds.Contains(vo.Id)).ToListAsync();
                foreach (var option in options)
                {
                    productItem.VariationOptions.Add(option);
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}
