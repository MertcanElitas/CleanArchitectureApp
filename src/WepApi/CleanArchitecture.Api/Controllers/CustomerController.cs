using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Web.Http;

namespace CleanArchitecture.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiVersion("1.0")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController (ICustomerService customerService)
        {
            _customerService = customerService;
        }


        [HttpGet("allproducts")]
        public async Task<IActionResult> GetProducts()
        {
            var data = await _customerService.GetCustomerList();
            return Ok(data);
        }

        [HttpGet("insert-bulk-customers")]
        public async Task<IActionResult> InsertBulkCustomer()
        {
            var data = await _customerService.InsertBulkCustomerIndex();
            return Ok(data);
        }

        [HttpGet("suggest")]
        public async Task<IActionResult> SuggestCustomer(string searchText)
        {
            var data = await _customerService.SuggestSearchAsync(searchText);
            return Ok(data);
        }

        [HttpGet("search-with-term")]
        public async Task<IActionResult> SearchWithTerm(string searchText)
        {
            var data = await _customerService.SearchWithTerm(searchText);
            return Ok(data);
        }

        [HttpGet("search-with-terms")]
        public async  Task<IActionResult> SearchWithTerms(int userId, int categoryId,
            string categoryName, string contactName)
        {
            var data = await _customerService.GetSearchAsyncWithTerms(userId, categoryId, categoryName, contactName);
            return Ok(data);
        }

        [HttpGet("search-with-terms-ids")]
        public async  Task<IActionResult> SearchWithTermsIds(string userIds)
        {
            var param = userIds.Split(",").Select(x => Convert.ToInt32(x)).ToList();
            var data = await _customerService.GetSearchAsyncWithTermsIds(param);
            return Ok(data);
        }

        [HttpGet("search-with-terms-multiple-ids")]
        public async  Task<IActionResult> SearchWithTermsIds(string userIds, string categoryIds = null)
        {
            var param = userIds?.Split(",").Select(x => Convert.ToInt32(x)).ToList();
            var param2 = categoryIds?.Split(",").Select(x => Convert.ToInt32(x)).ToList();
            var data = await _customerService.GetSearchAsyncWithTermsIds(param, param2);
            return Ok(data);
        }

        [HttpGet("search-with-terms-multiple-string")]
        public async  Task<IActionResult> SearchWithTermsByString(string country, string contactTitle = null)
        {
            var data = await _customerService.GetSearchAsyncWithTermsByString(country, contactTitle);
            return Ok(data);
        }

        [HttpGet("search-with-terms-multiple-bool")]
        public async  Task<IActionResult> SearchWithTermsByBool(string userIds, string categoryIds = null)
        {
            var param = userIds?.Split(",").Select(x => Convert.ToInt32(x)).ToList();
            var param2 = categoryIds?.Split(",").Select(x => Convert.ToInt32(x)).ToList();
            var data = await _customerService.GetSearchAsyncWithTermsMultipleIdsV2(param, param2);
            return Ok(data);
        }
    }
}