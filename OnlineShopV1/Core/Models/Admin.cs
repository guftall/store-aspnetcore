using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace OnlineShopV1
{
    public class Admin
    {
        [Key]
        public int ID { get; set; }
        
        [MinLength(5)]
        public String Username { get; set; }

        [MinLength(5)]
        [Required]
        public String Password { get; set; }
        
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Login Date")]
        public DateTime LastLogin { get; set; }

        public string HashPassword()
        {
            var hasher = new PasswordHasher<Admin>();
            Password = hasher.HashPassword(null, Password);
            return Password;
        }

        public bool VerifyPassword(string plain)
        {
            var hasher = new PasswordHasher<Admin>();
            return hasher.VerifyHashedPassword(null, Password, plain) == PasswordVerificationResult.Success;
        }
        
        public void UpdateLoginDate()
        {
            LastLogin = DateTime.Now;
        }
    }
    
    
    public interface IAdminRepository
    {
        Task<Admin> GetByUsername(string username);
        Task<Admin> GetByID(int id);
        Task Update(Admin admin);
    }
    
}