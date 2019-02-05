using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlineShopV1.Core.Interfaces;

namespace OnlineShopV1.DAL
{
    public class AuthenticationRepository : IAuthenticationRepository
    {

        private readonly OnlineShopDbContext _context;
        public AuthenticationRepository(OnlineShopDbContext context)
        {
            _context = context;
        }
        public Task<Authentication> GetByCode(string code)
        {
            return _context.Authentications.SingleOrDefaultAsync(a => a.Code == code);
        }

        public Task<Authentication> GetByUsername(string username)
        {
            return _context.Authentications.SingleOrDefaultAsync(a => a.Username == username);
        }

        public Task AddAuthentication(Authentication authentication)
        {
            _context.Authentications.Add(authentication);
            return _context.SaveChangesAsync();
        }

        public Task UpdateAuthentication(Authentication authentication)
        {
            _context.Authentications.Update(authentication);
            return _context.SaveChangesAsync();
        }
    }
}