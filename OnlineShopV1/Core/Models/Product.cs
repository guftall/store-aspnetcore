using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace OnlineShopV1
{
    public class Product
    {
        [Key]
        public int ID { get; set; }
        
        [Required]    
        public String Name { get; set; }
    }
    
    
    public interface IProductRepository
    {

        Task<List<Product>> ListProducts();
        Task<Product> GetByIdAsync(int id);
        Task AddAsync(Product product);
    }
}