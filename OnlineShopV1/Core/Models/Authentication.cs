using System;
using System.ComponentModel.DataAnnotations;
using MySql.Data.EntityFrameworkCore.DataAnnotations;
using OnlineShopV1.Core;

namespace OnlineShopV1
{
    [MySqlCharset("utf8mb4")]
    public class Authentication
    {
        [Key]
        public int ID { get; set; }
        
        [Required]
        public UserType UserType { get; set; }
        
        [Required]
        public String Code { get; set; }
        public string Username { get; set; } // admin username
        
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Expires Date")]
        public DateTime Expires { get; set; }

        public bool IsExpired()
        {
            return Expires < DateTime.Now;
        }

        public string GenerateNewCode()
        {
            Code = Utilities.GenerateAuthCode();
            return Code;
        }

        public void SetExpiresFromNow(int hours)
        {
            Expires = DateTime.Now.Add(TimeSpan.FromHours(hours));
        }
    }
}