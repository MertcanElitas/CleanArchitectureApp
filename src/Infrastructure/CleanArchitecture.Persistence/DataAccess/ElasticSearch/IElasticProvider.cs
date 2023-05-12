using System.Threading.Tasks;
using CleanArchitecture.Domain.Entities;
using Nest;

namespace CleanArchitecture.Persistence.DataAccess.ElasticSearch
{
    public interface IElasticProvider
    {
        public IElasticClient ElasticClient { get; set; }
        Task ChekIndex<T>(string indexName, string aliasName, int shardCount, int replicaCount) where T : ElasticEntity;

        Task<ISearchResponse<T>> SimpleSearchAsync<T>(string indexName, SearchDescriptor<T> query)
            where T : ElasticEntity;

        Task<ISearchResponse<T>> SearchAsync<T, TKey>(string indexName, SearchDescriptor<T> query,
            int skip, int size, string[] includeFields = null,
            string preTags = "<strong style=\"color: red;\">", string postTags = "</strong>",
            bool disableHigh = false, params string[] highField) where T : ElasticEntity;
    }
}