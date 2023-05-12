using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Nest;

namespace CleanArchitecture.Persistence.DataAccess.ElasticSearch.Repository
{
    public class CategoryElasticRepository : ICategoryElasticRepository
    {
        private readonly IElasticClient _elasticClient;
        private readonly string _indexName;
        private readonly string _aliasName;

        public CategoryElasticRepository(IElasticProvider elasticProvider, IConfiguration _configuration)
        {
            _elasticClient = elasticProvider.ElasticClient;
            _indexName = _configuration.GetSection("ElasticSearchSettings:CustomerIndexName").Value;
            _aliasName = _configuration.GetSection("ElasticSearchSettings:CustomerAliasName").Value;
            elasticProvider.ChekIndex<Customer>(_indexName, _aliasName , 3, 1).Wait();
        }

        public async Task<bool> CreateIndexAsync(string indexName)
        {
            var createIndexDescriptor = new CreateIndexDescriptor(indexName)
                .Mappings(ms => ms
                    .Map<Category>(m => m
                        .AutoMap()
                        .Properties(ps => ps
                            .Completion(c => c
                                .Name(p => p.Suggest))))
                );

            if (_elasticClient.Indices.Exists(indexName.ToLowerInvariant()).Exists)
            {
                _elasticClient.Indices.Delete(indexName.ToLowerInvariant());
            }

            CreateIndexResponse createIndexResponse = await _elasticClient.Indices.CreateAsync(createIndexDescriptor);

            return createIndexResponse.IsValid;
        }

        public async Task IndexAsync(string indexName, List<Category> categories)
        {
            await _elasticClient.IndexManyAsync(categories, indexName);
        }

        public async Task<CategorySuggestResponse> SuggestAsync(string indexName, string keyword)
        {
            ISearchResponse<Category> searchResponse = await _elasticClient.SearchAsync<Category>(s => s
                .Index(indexName)
                .Suggest(su => su
                    .Completion("suggestions", c => c
                        .Field(f => f.Suggest)
                        .Prefix(keyword)
                        .Fuzzy(f => f
                            .Fuzziness(Fuzziness.Auto)
                        )
                        .Size(5))
                ));

            var suggests = from suggest in searchResponse.Suggest["suggestions"]
                from option in suggest.Options
                select new CategorySuggest
                {
                    Id = option.Source.Id,
                    Name = option.Source.Name,
                    SuggestedName = option.Text,
                    Score = option.Score
                };

            return new CategorySuggestResponse
            {
                Suggests = suggests
            };
        }
    }
}