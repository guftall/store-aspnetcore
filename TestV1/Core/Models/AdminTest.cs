using System.Threading.Tasks;
using OnlineShopV1;
using Xunit;

namespace TestV1.Models
{
    public class AdminTest
    {
        [Fact]
        public void HashPasswordAndVerifyIt()
        {

            const string hardCodePass = "12345o";
            var admin = new Admin();
            admin.Password = hardCodePass;
            admin.HashPassword();
            
            Assert.True(admin.VerifyPassword(hardCodePass));
        }
    }
}