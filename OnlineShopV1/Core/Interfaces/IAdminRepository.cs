using System.Threading.Tasks;

namespace OnlineShopV1.Core.Interfaces
{
    public interface IAdminRepository
    {
        Task<Admin> GetByUsername(string username);
        Task<Admin> GetByID(int id);
        Task Update(Admin admin);
    }
}