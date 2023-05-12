using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Nest;

namespace CleanArchitecture.Persistence.DataAccess.ElasticSearch.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IElasticClient _elasticClient;
        private readonly string _indexName;
        private readonly string _aliasName;

        public CustomerRepository (IElasticProvider elasticProvider, IConfiguration _configuration)
        {
            _elasticClient = elasticProvider.ElasticClient;
            _indexName = _configuration.GetSection("ElasticSearchSettings:CustomerIndexName").Value;
            _aliasName = _configuration.GetSection("ElasticSearchSettings:CustomerAliasName").Value;
            elasticProvider.ChekIndex<Customer>(_indexName, _aliasName , 3, 1).Wait();
        }

        public async Task<IEnumerable<Customer>> GetCustomers()
        {
            var searchResponse = await _elasticClient
                .SearchAsync<Customer>(s => s.Index(_indexName)
                    .Query(q => q.MatchAll()));

            return searchResponse.IsValid ? searchResponse.Documents.ToList() : default;
        }

        public async Task<Customer> GetById(int id)
        {
            var response = await _elasticClient.GetAsync<Customer>(id, g => g.Index(_indexName));
            return response.Source;
        }

        public async Task<bool> Insert(Customer customer, bool isCommit = false)
        {
            var indexResponse = await _elasticClient.IndexDocumentAsync(customer);

            return indexResponse.IsValid;
        }

        public async Task<bool> BulkInsert(List<Customer> customers, bool isCommit = false)
        {
            var asyncBulkIndexResponse = _elasticClient.Bulk(b => b
                .Index(_indexName)
                .IndexMany(customers)
            );

            return !asyncBulkIndexResponse.Errors;
        }
        
     

        public async Task<bool> Delete(Customer customer, bool isCommit = false)
        {
            return await DeleteById(customer.Id);
        }

        public async Task<bool> DeleteById(int id, bool isCommit = false)
        {
            var response = await _elasticClient.DeleteAsync<Customer>(id, d => d.Index(_indexName));
            return response.IsValid;
        }

        public Task<bool> SaveChangeAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}