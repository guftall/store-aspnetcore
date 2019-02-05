using System.Collections.Generic;
using System.Threading.Tasks;

namespace OnlineShopV1.Core.Interfaces
{
    public interface IProductRepository
    {

        Task<List<Product>> ListProducts();
        Task<Product> GetByIdAsync(int id);
        Task AddAsync(Product product);
    }
}