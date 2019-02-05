using System.Threading.Tasks;

namespace OnlineShopV1.Core.Interfaces
{
    public interface IAuthenticationRepository
    {
        Task<Authentication> GetByCode(string code);
        Task<Authentication> GetByUsername(string username);
        Task AddAuthentication(Authentication authentication);
        Task UpdateAuthentication(Authentication authentication);
    }
}