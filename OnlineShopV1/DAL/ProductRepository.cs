using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlineShopV1.Core.Interfaces;

namespace OnlineShopV1.DAL
{
    public class ProductRepository : IProductRepository
    {

        private readonly OnlineShopDbContext _context;
        public ProductRepository(OnlineShopDbContext context)
        {
            _context = context;
        }

        public Task<List<Product>> ListProducts()
        {
            return _context.Products.ToListAsync();
        }
        public Task<Product> GetByIdAsync(int id)
        {
            return _context.Products.SingleOrDefaultAsync(p => p.ID == id);
        }

        public Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            return _context.SaveChangesAsync();
        }


        public static Product GetTestProduct()
        {

            var product = new Product
            {
                ID = 1
            };

            return product;
        }
    }
}