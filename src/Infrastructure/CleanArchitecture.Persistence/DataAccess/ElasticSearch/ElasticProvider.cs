using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Nest;
using Newtonsoft.Json;

namespace CleanArchitecture.Persistence.DataAccess.ElasticSearch
{
    public class ElasticProvider : IElasticProvider
    {
        public IElasticClient ElasticClient { get; set; }
        private readonly IConfiguration _configuration;

        public ElasticProvider (IConfiguration configuration)
        {
            _configuration = configuration;

            string host = _configuration.GetSection("ElasticSearchSettings:host").Value;
            string port = _configuration.GetSection("ElasticSearchSettings:port").Value;

            //DisablePing : Ayağa kalkan yeni node'lara veya daha önceden ölmüş olan node'lara initial ping atmasını engellemek için kapatılır. Amaç performası arttırmaktır.
            //SniffOnStartup :  İlk connection’ın çekilme anında, havuzun kontrol edilmesini engeller. Amaç performanstır.
            //SniffOnConnectionFault : Connection pool reconnection destekliyorsa, bir arama başarısız olduğunda ilgili connection havuzundan yeniden denetlenmesini engeller. Amaç performanstır.
            //DisableDirectStreaming : Bunu elasticsearch’de hata alındığı zaman daha detaylı hatayı alabilmek adına eklenmiştir. Memoryde performans kaybına neden olabilir. Sadece ihtiyaç anında kullanılmalıdır
            var settings = new ConnectionSettings(new Uri(host + ":" + port))
                .DisablePing()
                .DisableDirectStreaming()
                .SniffOnStartup(false)
                .SniffOnConnectionFault(false);

            ElasticClient = new ElasticClient(settings);
        }

        public async Task ChekIndex<T>(string indexName, string aliasName, int shardCount, int replicaCount)
            where T : ElasticEntity
        {
            var anyy = await ElasticClient.Indices.ExistsAsync(indexName);
            if (anyy.Exists)
                return;

            // var response = await ElasticClient.Indices.CreateAsync(indexName,
            //     ci => ci
            //         .Index(indexName)
            //         .Aliases(x => x.Alias(aliasName))
            //         .Settings(s => s.NumberOfShards(shardCount).NumberOfReplicas(replicaCount)));

            //max_result_window : Bu index üzerine yapılan sorguların from + size boyutu max default 1000 olarak belirlenir. Ama biz aşağıda int.MaxValue olarak set ettik.
            //my_ascii_folding : Temel Latin Unicode bloğunda olmayan alfabetik, sayısal ve sembolik karakterleri (ilk 127 ASCII karakteri), varsa ASCII eşdeğerlerine dönüştürür. Örneğin, filtre à - a olarak değişir.
            //analyzer olarak turkish - lowarcase analyer eklenmiş
            //turkish_analyzer : bu analyzer ile gelen türkçe karakterler default olarak ingilizce karakterlere dönüştürülmez. şafak => safak olarak dönüşmez bizde token ları bozmadan kaydetmiş oluruz.
            //standard tokenizer : En çok kullanılan tokenizer’lar dan biridir. Çoğu noktalama işaretini kaldırır. Kelimeleri aralarındaki boşluklara göre parçalar inverted index algoritması için idealdir.
            var response = await ElasticClient
                .Indices.CreateAsync(indexName,
                    ci => ci
                        .Index(indexName)
                        .Aliases(x => x.Alias(aliasName))
                        .Settings(s => s.NumberOfShards(shardCount).NumberOfReplicas(replicaCount)
                            .Setting("max_result_window", int.MaxValue)
                            .Analysis(anly => anly
                                .TokenFilters(tkf =>
                                    tkf.AsciiFolding("my_ascii_folding", af => af.PreserveOriginal(true)))
                                .Analyzers(aa => aa.Custom("turkish_analyzer",
                                    ca => ca.Filters("lowercase", "my_ascii_folding")
                                        .Tokenizer("standard"))))).Mappings(m => m.Map<T>(mm => mm.AutoMap()
                            .Properties(p => p
                                .Text(t => t.Name(n => n.SearchingArea)
                                    .Analyzer("turkish_analyzer")
                                )))));

            if (response.Acknowledged)
            {
                return;
            }

            throw new ElasticSearchException($"Create Index {indexName} failed : :" +
                                             response.ServerError.Error.Reason);
        }

        public virtual async Task<ISearchResponse<T>> SimpleSearchAsync<T>(string indexName, SearchDescriptor<T> query)
            where T : ElasticEntity
        {
            query.Index(indexName);
            var response = await ElasticClient.SearchAsync<T>(query);
            return response;
        }

        public virtual async Task<ISearchResponse<T>> SearchAsync<T, TKey>(string indexName, SearchDescriptor<T> query,
            int skip, int size, string[] includeFields = null,
            string preTags = "<strong style=\"color: red;\">", string postTags = "</strong>",
            bool disableHigh = false, params string[] highField) where T : ElasticEntity
        {
            query.Index(indexName);
            var highdes = new HighlightDescriptor<T>();
            if (disableHigh)
            {
                preTags = "";
                postTags = "";
            }

            highdes.PreTags(preTags).PostTags(postTags);

            var ishigh = highField != null && highField.Length > 0;

            var hfs = new List<Func<HighlightFieldDescriptor<T>, IHighlightField>>();

            //Pagination
            query.Skip(skip).Take(size);
            //Keyword highlighting
            if (ishigh)
            {
                foreach (var s in highField)
                {
                    hfs.Add(f => f.Field(s));
                }
            }

            highdes.Fields(hfs.ToArray());
            query.Highlight(h => highdes);
            if (includeFields != null)
                query.Source(ss => ss.Includes(ff => ff.Fields(includeFields.ToArray())));


            var data = JsonConvert.SerializeObject(query);
            var response = await ElasticClient.SearchAsync<T>(query);


            return response;
        }
    }
}