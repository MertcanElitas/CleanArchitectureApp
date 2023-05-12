using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitecture.Application.Dtos;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Interfaces.Services
{
    public interface ICustomerService
    {
        Task<IEnumerable<CustomerListModel>> GetCustomerList();
        Task<List<CustomerElasticIndex>> SuggestSearchAsync(string searchText);
        Task<List<CustomerElasticIndex>> GetSearchAsyncWithTermsIds(List<int> userIds);

        Task<List<CustomerElasticIndex>> GetSearchAsyncWithTerms(int userId, int categoryId,
            string categoryName, string contactName);

        Task<List<CustomerElasticIndex>> GetSearchAsyncWithTermsMultipleIdsV2(List<int> userIds,
            List<int> categoryIds);

        Task<List<CustomerElasticIndex>> GetSearchAsyncWithTermsIds(List<int> userIds,
            List<int> categoryIds);

        Task<List<CustomerElasticIndex>> GetSearchAsyncWithTermsByString(string country,
            string contactTitle);

        Task<bool> InsertBulkCustomer();
        Task<List<CustomerElasticIndex>> SearchWithTerm(string searchText);
        Task<bool> InsertBulkCustomerIndex();
    }
}