using System;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopV1
{
    public class Product
    {
        [Key]
        public int ID { get; set; }
        
        [Required]    
        public String Name { get; set; }
    }
}