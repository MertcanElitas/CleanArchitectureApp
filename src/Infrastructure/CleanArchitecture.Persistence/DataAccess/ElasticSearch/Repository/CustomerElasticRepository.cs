using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Nest;
using Newtonsoft.Json;

namespace CleanArchitecture.Persistence.DataAccess.ElasticSearch.Repository
{
    public class CustomerElasticRepository : ICustomerElasticRepository
    {
        private readonly IElasticClient _elasticClient;
        private readonly IElasticProvider _elasticProvider;
        private readonly string _indexName;
        private readonly string _aliasName;

        public CustomerElasticRepository (IElasticProvider elasticProvider, IConfiguration _configuration)
        {
            _elasticProvider = elasticProvider;
            _elasticClient = elasticProvider.ElasticClient;
            _indexName = _configuration.GetSection("ElasticSearchSettings:CustomerIndexName").Value;
            _aliasName = _configuration.GetSection("ElasticSearchSettings:CustomerAliasName").Value;
            elasticProvider.ChekIndex<Customer>(_indexName, _aliasName , 3, 1).Wait();
        }

        public async Task<List<CustomerElasticIndex>> SuggestSearchAsync(string searchText, int skipItemCount = 0,
            int maxItemCount = 5)
        {
            try
            {
                var queryy =
                    new Nest.SearchDescriptor<CustomerElasticIndex>()  // SearchDescriptor burada oluşturacağız 
                        .Suggest(su => su
                            .Completion("post_suggestions",
                                c => c.Field(f => f.Suggest)
                                    .Analyzer("simple")
                                    .Prefix(searchText)
                                    .Fuzzy(f => f.Fuzziness(Nest.Fuzziness.Auto))
                                    .Size(10))
                        );

                var returnData =
                    await _elasticProvider.SearchAsync<CustomerElasticIndex, int>(_indexName, queryy, skipItemCount,
                        maxItemCount );

                var data = JsonConvert.SerializeObject(returnData);

                if (returnData.Suggest.Keys.Count() > 0)
                {
                    var asd = returnData.Suggest["post_suggestions"];
                    if (asd != null && asd.Any())
                    {
                        foreach (var aa in asd)
                        {
                            foreach (var opt in aa.Options)
                            {
                                var fg = opt.Source?.Address;
                            }
                        }
                    }
                }

                var suggestsList = returnData.Suggest.Keys.Count() > 0
                    ? from suggest in returnData.Suggest["post_suggestions"]
                    from option in suggest.Options
                    select new CustomerElasticIndex
                    {
                        Score = option.Score,
                        City   = option.Source.City,
                        CompanyName   = option.Source.CompanyName,
                        Country = option.Source.Country,
                        Fax = option.Source.Fax,
                        Region = option.Source.Region,
                        Suggest = option.Source.Suggest,
                        Id = option.Source.Id
                    }
                    : null;

                return await Task.FromResult(suggestsList.ToList());
            }
            catch (Exception ex)
            {
                return await Task.FromException<List<CustomerElasticIndex>>(ex);
            }
        }

        public Task<List<CustomerElasticIndex>> GetSearchAsyncWithTerms(int userId, int categoryId, string categoryName,
            string contactName)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> BulkInsertCustomerElastic(List<CustomerElasticIndex> customers, bool isCommit = false)
        {
            var asyncBulkIndexResponse = _elasticClient.Bulk(b => b
                .Index(_indexName)
                .IndexMany(customers)
            );

            return !asyncBulkIndexResponse.Errors;
        }

        #region " Terms "

        public async Task<List<CustomerElasticIndex>> GetSearchAsyncWithTerm(string searchText, int skipItemCount = 0,
            int maxItemCount = 100)
        {
            var  searchQuery = new Nest.SearchDescriptor<CustomerElasticIndex>()
                .Query(q => q.Term(t => t.CategoryId, searchText));

            var searchResultData =
                await _elasticProvider.SimpleSearchAsync<CustomerElasticIndex>(_indexName, searchQuery);

            var result = GenerateResponse(searchResultData.Documents);

            return result;
        }

        //Terms sorgusu burada contains mantığı ile çalışır. Yani 20 tane userId gönderdiysek 
        //eşleşen 12 tane varsa 12 tane data döner
        public async Task<List<CustomerElasticIndex>> GetSearchAsyncWithTermsIds(List<int> userIds)
        {
            var    searchQuery = new Nest.SearchDescriptor<CustomerElasticIndex>()
                .Query(q =>
                    q.Terms(c => c
                        .Name("named_query")
                        .Boost(1.1)
                        .Field(p => p.UserId)
                        .Terms(userIds)
                    )
                );

            var searchResultData =
                await _elasticProvider.SimpleSearchAsync<CustomerElasticIndex>(_indexName, searchQuery);

            var result = GenerateResponse(searchResultData.Documents);

            return result;
        }

        //Aşağıdaki sorguda userId daha alt satırda olduğu için categoryId filteresi önemsiz duruma düşüp sadece userId'ye göre data döner
        // Nedenini anlamadım.
        public async Task<List<CustomerElasticIndex>> GetSearchAsyncWithTermsMultipleIds(List<int> userIds,
            List<int> categoryIds)
        {
            var    searchQuery = new Nest.SearchDescriptor<CustomerElasticIndex>()
                .Query(q =>
                    q.Terms(c => c
                        .Name("named_query")
                        .Boost(1.1)
                        .Field(p => p.CategoryId)
                        .Terms(categoryIds)
                        .Boost(1.1)
                        .Field(p => p.UserId)
                        .Terms(userIds)
                    )
                );

            var searchResultData =
                await _elasticProvider.SimpleSearchAsync<CustomerElasticIndex>(_indexName, searchQuery);

            var result = GenerateResponse(searchResultData.Documents);

            return result;
        }

        //Burdaki bool filtresi term ifadelerini bool operatörü gibi kullanmamızı sağlar.
        // örnek = userid = 5 ve category id=10 olan data.
        public async Task<List<CustomerElasticIndex>> GetSearchAsyncWithTermsMultipleIdsV2(List<int> userIds,
            List<int> categoryIds)
        {
            var    searchQuery = new Nest.SearchDescriptor<CustomerElasticIndex>()
                .Query(q => q
                    .Bool(bq => bq
                        .Filter(
                            fq => fq.Terms(t => t.Field(f => f.CategoryId).Terms(categoryIds)),
                            fq => fq.Terms(t => t.Field(f => f.UserId).Terms(userIds))
                        )));

            var searchResultData =
                await _elasticProvider.SimpleSearchAsync<CustomerElasticIndex>(_indexName, searchQuery);

            var result = GenerateResponse(searchResultData.Documents);

            return result;
        }

        public async Task<List<CustomerElasticIndex>> GetSearchAsyncWithTermsByString(string country,
            string contactTitle)
        {
            var    searchQuery = new Nest.SearchDescriptor<CustomerElasticIndex>()
                .Query(q =>
                    q.Terms(c => c
                        .Name("named_query")
                        .Boost(1.1)
                        .Field(p => p.Country)
                        .Terms(country)
                        .Boost(1.1)
                        .Field(p => p.ContactTitle)
                        .Terms(contactTitle)
                    )
                );

            var searchResultData =
                await _elasticProvider.SimpleSearchAsync<CustomerElasticIndex>(_indexName, searchQuery);

            var result = GenerateResponse(searchResultData.Documents);

            return result;
        }

        #endregion



        private List<CustomerElasticIndex> GenerateResponse(IReadOnlyCollection<CustomerElasticIndex> documents)
        {
            var result = new List<CustomerElasticIndex>();

            foreach (var opt in documents)
            {
                var model = new CustomerElasticIndex
                {
                    Score = opt.Score == null ? default : (double) opt.Score,
                    CategoryName = opt.CategoryName,
                    Title = opt.Title,
                    UserInfo = opt.UserInfo,
                    Suggest = opt.Suggest,
                    Url = opt.Url,
                    Id = opt.Id,
                    CategoryId = opt.CategoryId,
                    CreatedDate = opt.CreatedDate,
                    UserId = opt.UserId,
                    SearchingArea = opt.SearchingArea
                };

                result.Add(model);
            }

            return result;
        }



        public async Task<List<CustomerElasticIndex>> GetSearchAsync(string searchText, int skipItemCount = 0,
            int maxItemCount = 100)
        {
            try
            {
                var currentUserId = 1;
                var selectedCategoryId = 6;
                // search descriptor yazmak gerekiyor
                var indexName = _aliasName;
                var searchQuery =
                    new Nest.SearchDescriptor<
                        CustomerElasticIndex>(); /* new Nest.SearchDescriptor<PostElasticIndexDto>()
                            .Query(q =>
                                           //Termler: Sadece boolen yani “Yes / No” veya string bir kelime ile eşleşebilecek durumlarda kullanılır.
                                           // q.Term(t => t.UserId, currentUserId)   // tek bir parametreye ait sorgulama için
                                           // coklu term işlemleri birden cok parametreye ait sart işlemi için kullanılır.
                                           q.Terms(t => t
                                                         .Field(ff => ff.UserId).Terms<int>(currentUserId)
                                                         .Field(ff => ff.CategoryId).Terms<int>(selectedCategoryId))
                                     // aranan kelime veya cümle geçmesi yeterlidir, bire bir eşleme istemez
                                     && q.MatchPhrasePrefix(m => m.Field(f => f.SearchingArea).Query(searchText))
                                   // aranan kelime veya cümlenin bire bir eşleşmesi gerekmektedir.
                                   //|| q.MatchPhrase(m=> m.Field(f=> f.SearchingArea).Query(searchText))
                                   );*/

                //Termler: Sadece boolen yani “Yes / No” veya string bir kelime ile eşleşebilecek durumlarda kullanılır.
                searchQuery = new Nest.SearchDescriptor<CustomerElasticIndex>()
                    .Query(q => q.Term(t => t.UserId, currentUserId));

                //  // coklu term işlemleri birden cok parametreye ait sart işlemi için kullanılır.
                searchQuery = new Nest.SearchDescriptor<CustomerElasticIndex>()
                    .Query(q => q.Terms(t => t.Field(ff => ff.UserId).Terms<int>(currentUserId)
                        .Field(ff => ff.CategoryId).Terms<int>(selectedCategoryId))
                    );
                // aranan kelime veya cümle geçmesi yeterlidir, bire bir eşleme istemez
                searchQuery = new Nest.SearchDescriptor<CustomerElasticIndex>()
                    .Query(q => q.MatchPhrasePrefix(m => m.Field(f => f.SearchingArea).Query(searchText)));

                // aranan kelime veya cümlenin bire bir eşleşmesi gerekmektedir.
                searchQuery = new Nest.SearchDescriptor<CustomerElasticIndex>()
                    .Query(q => q.MatchPhrase(m => m.Field(f => f.SearchingArea).Query(searchText)));


                // komplex sorgular seklinde birleştirip yazabiliriz.
                searchQuery = new Nest.SearchDescriptor<CustomerElasticIndex>()
                    .Query(q => q.Terms(t => t.Field(ff => ff.UserId).Terms<int>(currentUserId)
                                    .Field(ff => ff.CategoryId).Terms<int>(selectedCategoryId)
                                )
                                && q.MatchPhrasePrefix(m => m.Field(f => f.SearchingArea).Query(searchText))
                    );


                //  arama kelimesi Core İşlemleri sql
                //https://www.elastic.co/guide/en/elasticsearch/reference/current/query-dsl-multi-match-query.html
                searchQuery = new Nest.SearchDescriptor<CustomerElasticIndex>()
                    .Query(q => q
                        .MultiMatch(m => m.Fields(f => f.Field(ff => ff.SearchingArea, 2.0)
                                    .Field(ff => ff.Title, 1.0)
                                )
                                .Query(searchText)
                                .Type(TextQueryType.BestFields)
                                .Operator(Operator.Or)  // Operator.And  dene 
                        )
                    );


                // 2.0 ve 1.0 ı vermeden yaz ve vererek yaz ama acıkla

                searchQuery = new Nest.SearchDescriptor<CustomerElasticIndex>()
                    .Query(q => q
                        .MultiMatch(m => m.Fields(f => f.Field(ff => ff.SearchingArea, 2.0)
                                .Field(ff => ff.Title, 1.0)
                            )
                            .Query(searchText)
                            .Type(TextQueryType.BestFields)
                            .Operator(Operator.Or)
                            .MinimumShouldMatch(3)
                        )
                    )
                    .Sort(s => s.Descending(f => f.CreatedDate));


                //searchQuery = new Nest.SearchDescriptor<PostElasticIndexDto>()
                //          .Query(q =>
                //                       q.MultiMatch(m => m.Fields(f => f.Field(ff => ff.SearchingArea, 2.0)
                //                                                       .Field(ff => ff.Title, 1.0)
                //                                                )
                //                                         .Query(searchText)
                //                                         .Type(TextQueryType.BestFields)
                //                                         .Operator(Operator.Or)  // Operator.And  dene 
                //                                   )
                //                    && q.Range(r => r.Field(rf => rf.TagNameValues.Count).GreaterThan(2))
                //                    || q.Range(r => r.Field(rf => rf.TagNameValues.Count).GreaterThanOrEquals(3))
                //               )
                //          .Sort(s => s.Descending(f => f.CreatedDate.Date));

                /*
                 *  https://www.elastic.co/guide/en/elasticsearch/painless/7.5/painless-operators-boolean.html
                 greater_than: expression '>' expression;
                 greater_than_or_equal: expression '>=' expression;
                 less_than: expression '<' expression;
                 greater_than_or_equal: expression '<=' expression;
                 instance_of: ID 'instanceof' TYPE;
                 equality_equals: expression '==' expression;
                 equality_not_equals: expression '!=' expression;
                 identity_equals: expression '===' expression;
                 identity_not_equals: expression '!==' expression;
                 boolean_xor: expression '^' expression;
                 boolean_and: expression '&&' expression;
                 boolean_and: expression '||' expression;
                 */

                /*
                 f.Field(ff => ff.SearchingArea, 2.0)  buradaki 2.0 işlemi boost işlemidir.
                 öncelik  ve katsayı işlemidir   ÖNCELİKLENDİRME İŞLEMİDİR.
                 */
                //searchQuery = new Nest.SearchDescriptor<PostElasticIndexDto>()
                //          .Query(q =>
                //                       q.MultiMatch(m => m.Fields(f => f.Field(ff => ff.SearchingArea, 2.0)
                //                                                       .Field(ff => ff.Title, 1.0)
                //                                                )
                //                                         .Query(searchText)
                //                                         .Type(TextQueryType.BestFields)
                //                                         .Operator(Operator.Or)  // Operator.And  dene 
                //                                   )
                //                    && q.Range(r => r.Field(rf => rf.TagNameValues.Count).GreaterThan(2))
                //                    && q.Range(r => r.Field(rf => rf.TagNameValues.Count).GreaterThanOrEquals(3))
                //               )
                //          .Sort(s => s.Descending(f => f.CreatedDate.Date))
                //          .Skip(skipItemCount)
                //          .Take(maxItemCount);


                //  searchQuery = new Nest.SearchDescriptor<PostElasticIndexDto>()
                //           .Query(q =>
                //q.Bool(b => b.Should(s => TermAny(s, "userCodeCores", userCodeList.ToArray())))
                //                        );


                //            searchQuery = new Nest.SearchDescriptor<PostElasticIndexDto>()
                //.Query(q => q
                //    .Bool(b => b
                //        .Should(
                //            bs => bs.Term(p => p.UserId, 1),
                //            bs => bs.Term(p => p.CategoryId, 5)
                //        ).MinimumShouldMatch(3)
                //    )
                //);
                /* bool tipinde sorgular
                
                 must=>Cümle (sorgu) eşleşen belgelerde görünmelidir ve skora katkıda bulunacaktır.
                 filter=> Yan tümce (sorgu) eşleşen belgelerde görünmelidir. Ancak zorunluluktan farklı olarak, sorgunun puanı dikkate alınmaz.
                 should=> Yan tümce (sorgu) eşleşen belgede görünmelidir. Zorunlu veya filtre yan tümcesi olmayan bir boole sorgusunda, bir veya daha fazla yan tümce, 
                          bir belgeyle eşleşmelidir. Eşleşmesi gereken minimum koşul cümlesi sayısı minimum_should_match parametresi kullanılarak ayarlanabilir.
                 must_not=> Yan tümce (sorgu) eşleşen belgelerde görünmemelidir.
                  */


                //searchQuery = new Nest.SearchDescriptor<PostElasticIndexDto>()
                //    .Query(q =>
                //                q.Bool(b => b
                //                            .MustNot(m => m.MatchAll())
                //                            .Should(m => m.MatchAll())
                //                            .Must(m => m.MatchAll())
                //                            .Filter(f => f.MatchAll())
                //                            .MinimumShouldMatch(1)
                //                            .Boost(2))
                //    );


                //QueryContainer qvw = new TermQuery { Field = "x", Value = "x" };
                //var xyz = Enumerable.Range(0, 1000).Select(f => qvw).ToArray();
                //var boolQuery = new BoolQuery
                //{
                //    Must = xyz
                //};

                //var c = new QueryContainer();
                //var qq = new TermQuery { Field = "x", Value = "x" };

                //for (var i = 0; i < 10; i++)
                //{
                //    c &= qq;
                //}


//                searchQuery = new Nest.SearchDescriptor<PostElasticIndexDto>().Query(q =>
//q.QueryString(qs =>
//qs.DefaultField(d => d.CategoryName).Query(" c# sql server ".Trim()).DefaultOperator(Operator.And)));


                //, 0, 10,null, "<strong style=\"color: red;\">", "</strong>", false, new string[] { "Title" }
                var searchResultData =
                    await _elasticProvider.SimpleSearchAsync<CustomerElasticIndex>(indexName, searchQuery);

                if (searchResultData.Hits.Count > 0)
                {
                    var data = JsonConvert.SerializeObject(searchResultData);
                }

                //var midir = from opt in searchResultData.Hits
                //            select new PostElasticIndexDto
                //            {
                //                Score = (double)opt.Score,
                //                CategoryName = opt.Source.CategoryName,
                //                Title = opt.Source.Title,
                //                UserInfo = opt.Source.UserInfo,
                //                Suggest = opt.Source.Suggest,
                //                Url = opt.Source.Url,
                //                Id = opt.Source.Id,
                //                CategoryId = opt.Source.CategoryId,
                //                CreatedDate = opt.Source.CreatedDate,
                //                UserId = opt.Source.UserId,
                //                TagNameValues = opt.Source.TagNameValues,
                //                TagNameIds = opt.Source.TagNameIds
                //            };


                var result2 = from opt in searchResultData.Documents
                    select new CustomerElasticIndex
                    {
                        Score = (double) opt.Score,
                        CategoryName = opt.CategoryName,
                        Title = opt.Title,
                        UserInfo = opt.UserInfo,
                        Suggest = opt.Suggest,
                        Url = opt.Url,
                        Id = opt.Id,
                        CategoryId = opt.CategoryId,
                        CreatedDate = opt.CreatedDate,
                        UserId = opt.UserId,
                        // TagNameValues = opt.TagNameValues,
                        // TagNameIds = opt.TagNameIds,
                        SearchingArea = opt.SearchingArea
                    };

                return await Task.FromResult<List<CustomerElasticIndex>>(result2.ToList());
            }
            catch (Exception ex)
            {
                return await Task.FromException<List<CustomerElasticIndex>>(ex);
            }
        }
    }
}