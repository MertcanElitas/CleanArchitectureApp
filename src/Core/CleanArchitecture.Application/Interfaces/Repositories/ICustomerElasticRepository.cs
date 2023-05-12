using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitecture.Domain.Entities;
using Nest;

namespace CleanArchitecture.Application.Interfaces.Repositories
{
    public interface ICustomerElasticRepository
    {
        Task<List<CustomerElasticIndex>> SuggestSearchAsync(string searchText, int skipItemCount = 0,
            int maxItemCount = 5);

        Task<List<CustomerElasticIndex>> GetSearchAsyncWithTerm(string searchText, int skipItemCount = 0,
            int maxItemCount = 100);

        Task<List<CustomerElasticIndex>> GetSearchAsyncWithTermsIds(List<int> userIds);

        Task<List<CustomerElasticIndex>> GetSearchAsyncWithTermsMultipleIds(List<int> userIds,
            List<int> categoryIds);

        Task<List<CustomerElasticIndex>> GetSearchAsyncWithTermsByString(string country, string contactTitle);

        Task<List<CustomerElasticIndex>> GetSearchAsyncWithTermsMultipleIdsV2(List<int> userIds,
            List<int> categoryIds);

        Task<List<CustomerElasticIndex>> GetSearchAsyncWithTerms(int userId, int categoryId,
            string categoryName, string contactName);

        Task<bool> BulkInsertCustomerElastic(List<CustomerElasticIndex> customers, bool isCommit = false);
    }
}