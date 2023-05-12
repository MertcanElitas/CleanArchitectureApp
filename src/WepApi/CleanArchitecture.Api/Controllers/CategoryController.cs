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
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController (ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }


        [HttpGet("insert-cagetories")]
        public async Task<IActionResult> BulkInsert()
        {
            var data = await _categoryService.BulkInsert();
            return Ok(data);
        }

        [HttpGet("suggest")]
        public async Task<CategorySuggestModel> Get(string keyword)
        {
            return await _categoryService.Suggest( keyword);
        }
    }
}