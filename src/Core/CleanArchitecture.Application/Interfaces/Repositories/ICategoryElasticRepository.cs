using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Interfaces.Repositories
{
    public interface ICategoryElasticRepository
    {
        Task<bool> CreateIndexAsync(string indexName);
        Task IndexAsync(string indexName, List<Category> categories);
        Task<CategorySuggestResponse> SuggestAsync(string indexName, string keyword);
    }
}