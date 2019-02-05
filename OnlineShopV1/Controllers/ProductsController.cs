using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OnlineShopV1.Core.Interfaces;
using OnlineShopV1.Core.Responses;
using StatusCodes = Microsoft.AspNetCore.Http.StatusCodes;

namespace OnlineShopV1.Controllers
{
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {

        private readonly IProductRepository _repository;

        public ProductsController(IProductRepository repository)
        {

            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<ProductsListResponse>> ListProducts()
        {
            var result = await _repository.ListProducts();

            return Ok(new ProductsListResponse(result));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResponse>> GetProduct(int id)
        {

            var prod = await _repository.GetByIdAsync(id);
            if (prod == null)
            {
                return NotFound(new MNotFound());
            }

            return Ok(new ProductResponse(prod));
        }

        [HttpPost]
        public async Task<ActionResult<DefaultResponse>> AddProduct([FromBody] Product prod)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(new MBadRequest(ModelState));
            }
            await _repository.AddAsync(prod);
            return Ok(new DefaultResponse());
        }
    }
}