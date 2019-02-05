using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlineShopV1.Core.Interfaces;

namespace OnlineShopV1.DAL
{
    public class AdminRepository : IAdminRepository
    {

        private readonly OnlineShopDbContext _context;
        public AdminRepository(OnlineShopDbContext context)
        {
            _context = context;
        }
        public Task<Admin> GetByUsername(string username)
        {
            return _context.Admins.SingleOrDefaultAsync(a => a.Username == username);
        }

        public Task<Admin> GetByID(int id)
        {
            return _context.Admins.SingleOrDefaultAsync(a => a.ID == id);
        }
    }
}