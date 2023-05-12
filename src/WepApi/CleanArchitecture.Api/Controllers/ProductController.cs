using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitecture.Application.Dtos;
using CleanArchitecture.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Web.Http;

namespace CleanArchitecture.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController (IProductService productService)
        {
            _productService = productService;
        }

        // GET
        [HttpGet("allproducts")]
        public async Task<IActionResult> GetProducts()
        {
            var data = await _productService.GetProductList();
            return Ok(data);
        }
    }
}