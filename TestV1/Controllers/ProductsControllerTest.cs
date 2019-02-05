using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OnlineShopV1.Controllers;
using OnlineShopV1.Core.Interfaces;
using OnlineShopV1.Core.Responses;
using OnlineShopV1.DAL;
using Xunit;

namespace TestV1
{
    public class ProductsControllerTest
    {
        [Fact]
        public async Task GetProduct_ReturnProducts()
        {
            const int testProdID = 1234;
            var testProd = ProductRepository.GetTestProduct();
            var mockRepo = new Mock<IProductRepository>();
            mockRepo.Setup(repo => repo.GetByIdAsync(testProdID))
                .ReturnsAsync(testProd);

            var controller = new ProductsController(mockRepo.Object);

            var result = await controller.GetProduct(testProdID);
            
            var actionResult = Assert.IsType<ActionResult<ProductResponse>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var response = Assert.IsType<ProductResponse>(okResult.Value);
            Assert.Equal(testProd.ID, response.Product.ID);
        }
        
        [Fact]
        public async Task GetProduct_ReturnNotFound_IdNotFound()
        {
            const int testProdID = 1234;
            var mockRepo = new Mock<IProductRepository>();

            var controller = new ProductsController(mockRepo.Object);

            var result = await controller.GetProduct(testProdID);
            
            Assert.IsType<ActionResult<ProductResponse>>(result);
            var objResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            var response = Assert.IsType<MNotFound>(objResult.Value);
            Assert.Equal((int) StatusCodes.NotFound, response.status);
        }

        [Fact]
        public async Task AddProduct_ReturnBadRequest_EmptyRequest()
        {

            var mockRepo = new Mock<IProductRepository>();
            var controller = new ProductsController(mockRepo.Object);

            controller.ModelState.AddModelError("", "empty body");

            var result = await controller.AddProduct(null);

            Assert.IsType<ActionResult<DefaultResponse>>(result);
            var okResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            var response = Assert.IsType<MBadRequest>(okResult.Value);
            Assert.Equal((int) StatusCodes.BadRequest, response.status);
        }
    }
}