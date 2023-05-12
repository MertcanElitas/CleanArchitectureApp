using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Application.Interfaces;
using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Application.Interfaces.Services;
using CleanArchitecture.Domain.Entities;
using Nest;

namespace CleanArchitecture.Application.Services
{
    public class ActorService : IActorService
    {
        private readonly IActorRepository _actorsRepository;

        public ActorService(IActorRepository actorsRepository)
        {
            _actorsRepository = actorsRepository;
        }

        public async Task InsertManyAsync()
        {
            await _actorsRepository.InsertManyAsync(NestExtensions.GetSampleData());
        }

        public async Task<ICollection<Actors>> GetAllAsync()
        {
            var result = await _actorsRepository.GetAllAsync();

            return result.ToList();
        }

        //lowcase
        public async Task<ICollection<Actors>> GetByNameWithTerm(string name)
        {
            var query = new QueryContainerDescriptor<Actors>().Term(p =>
                p.Field(p => p.Name).Value(name).CaseInsensitive().Boost(6.0));
            var result = await _actorsRepository.SearchAsync(_ => query);

            return result?.ToList();
        }

        //using operator OR in case insensitive
        public async Task<ICollection<Actors>> GetByNameWithMatch(string name)
        {
            var query = new QueryContainerDescriptor<Actors>()
                .Match(p =>
                    p.Field(f => f.Name).Query(name)
                        .Operator(Operator.And));
            var result = await _actorsRepository.SearchAsync(_ => query);

            return result?.ToList();
        }


        public async Task<ICollection<Actors>> GetByNameWithMatchAndAge(string name, int age)
        {
            //And query'si dir. Name = mertcan ve Age=5 olan datayı döner. 1 condition bile sağlanmazsa count 0 döner.
            var mustquery = new QueryContainerDescriptor<Actors>().Bool(b => b
                .Must(mu => mu
                        .Match(m => m
                            .Field(f => f.Name)
                            .Query(name)
                        ), mu => mu
                        .Match(m => m
                            .Field(f => f.Age)
                            .Query(age.ToString())
                        )
                )
            );


            //OR query'si dir. Name = mertcan veya Age=5 olan datayı döner. 1 condition bile sağlarsa count 1 döner.
            var shouldquery = new QueryContainerDescriptor<Actors>().Bool(b => b
                .Should(mu => mu
                        .Match(m => m
                            .Field(f => f.Name)
                            .Query(name)
                        ), mu => mu
                        .Match(m => m
                            .Field(f => f.Age)
                            .Query(age.ToString())
                        )
                )
            );

            var queryTerm = new QueryContainerDescriptor<Actors>();

            // Bu şekilde complext sorgular && ve || operatörleri ile yazılabilir. Aşağıdaki query şu anlama gelir
            // Description alanında cada dez ifadesi geçen ve total movie değeri 50 den büyük olan ve age değeri 45 den büyük olan yada age değeri 80 olan datayı getir.
            var complexConditions =
                ((queryTerm.Match(m => m.Field(f => f.Description).Query("cada dez"))  &&
                  queryTerm.Range(c => c.Field(p => p.TotalMovies).GreaterThan(50)) &&
                  queryTerm.Range(c => c.Field(p => p.Age).GreaterThan(45))) ||
                 queryTerm.Match(ma => ma.Field(f => f.Age).Query("80")));
            

            var mustresult = await _actorsRepository.SearchAsync(_ => mustquery);
            var shouldresult = await _actorsRepository.SearchAsync(_ => shouldquery);
            var complexConditionResult = await _actorsRepository.SearchAsync(_ => complexConditions);

            return mustresult?.ToList();
        }
        
        //Genel olarak text ifadelerinin search'ü için kullanılır. Aratılan ifadenin metin içerisinde birebir eşleşmiş olarak geçmesi gerekir.
        //Örnek olarak Mertcan Elitaş Okula Gitti. İfadesini aratırsak Mertcan yazarsak sonuç döner Elitaş yazarsak sonuç döner Gitti yazarsak sonuç döner
        //ama Elitaş yerine Elit yazarsak sonuç dönmez.
        public async Task<ICollection<Actors>> GetByNameWithMatchPhrase(string name)
        {
            var query = new QueryContainerDescriptor<Actors>()
                .MatchPhrase(p => p.
                    Field(f => f.Name).
                    Query(name));
            
            var result = await _actorsRepository.SearchAsync(_ => query);

            return result?.ToList();
        }

        //Genel olarak text ifadelerinin search'ü için kullanılır. Aratılan ifadenin metin içerisinde birebir eşleşmiş olarak geçmesi gerekmez contains mantığında çalışır.
        //Örnek olarak Mertcan Elitaş Okula Gitti. İfadesini aratırsak Mertcan yazarsak sonuç döner Elitaş yazarsak sonuç döner Gitti yazarsak sonuç döner Elitaş yerine Elit yazarsak sonuç yine döner.
        public async Task<ICollection<Actors>> GetByNameWithMatchPhrasePrefix(string name)
        {
            var query = new QueryContainerDescriptor<Actors>().MatchPhrasePrefix(p =>
                p.Field(f => f.Name).Query(name));
            var result = await _actorsRepository.SearchAsync(_ => query);

            return result?.ToList();
        }

        //contains
        public async Task<ICollection<Actors>> GetByNameWithWildcard(string name)
        {
            var query = new QueryContainerDescriptor<Actors>().Wildcard(w =>
                w.Field(f => f.Name).Value($"*{name}*").CaseInsensitive());
            var result = await _actorsRepository.SearchAsync(_ => query);

            return result?.ToList();
        }

        public async Task<ICollection<Actors>> GetByNameWithFuzzy(string name)
        {
            var query = new QueryContainerDescriptor<Actors>()
                .Fuzzy(descriptor =>
                descriptor
                    .Boost(6.0)
                    .Field(p => p.Name)
                    .Value(name));
            var result = await _actorsRepository.SearchAsync(_ => query);

            return result?.ToList();
        }

        public async Task<ICollection<Actors>> SearchInAllFiels(string term)
        {
            var query = NestExtensions.BuildMultiMatchQuery<Actors>(term);
            var result = await _actorsRepository.SearchAsync(_ => query);

            return result.ToList();
        }

        public async Task<ICollection<Actors>> GetByDescriptionMatch(string description)
        {
            //case insensitive
            var query = new QueryContainerDescriptor<Actors>().Match(p =>
                p.Field(f => f.Description).Query(description));
            var result = await _actorsRepository.SearchAsync(_ => query);

            return result?.ToList();
        }

        public async Task<ICollection<Actors>> GetByNameAndDescriptionMultiMatch(string term)
        {
            var query = new QueryContainerDescriptor<Actors>()
                .MultiMatch(p =>
                    p.Fields(p => p.Field(f => f.Name).Field(d => d.Description)).Query(term).Operator(Operator.And));

            var result = await _actorsRepository.SearchAsync(_ => query);

            return result?.ToList();
        }

        // Name ve Description alanları null değilse ve querysi olarak ekler datetime alanı null değilse de 
        // onuda ve koşulu olarak ekler.
        public async Task<ICollection<Actors>> GetActorsCondition(string name, string description,
            DateTime? birthdate)
        {
            QueryContainer query = new QueryContainerDescriptor<Actors>();

            if (!string.IsNullOrEmpty(name))
            {
                query = query &&
                        new QueryContainerDescriptor<Actors>().MatchPhrasePrefix(qs =>
                            qs.Field(fs => fs.Name).Query(name));
            }

            if (!string.IsNullOrEmpty(description))
            {
                query = query &&
                        new QueryContainerDescriptor<Actors>().MatchPhrasePrefix(qs =>
                            qs.Field(fs => fs.Description).Query(description));
            }

            if (birthdate.HasValue)
            {
                query = query && new QueryContainerDescriptor<Actors>()
                    .Bool(b => b.Filter(f => f.DateRange(dt => dt
                        .Field(field => field.BirthDate)
                        .GreaterThanOrEquals(birthdate)
                        .LessThanOrEquals(birthdate)
                        .TimeZone("+00:00"))));
            }

            var result = await _actorsRepository.SearchAsync(_ => query);

            return result?.ToList();
        }

        public async Task<ICollection<Actors>> GetActorsAllCondition(string term)
        {
            var query = new QueryContainerDescriptor<Actors>().Bool(b =>
                b.Must(m => m.Exists(e => e.Field(f => f.Description))));
            int.TryParse(term, out var numero);

            query = query && new QueryContainerDescriptor<Actors>().Wildcard(w =>
                        w.Field(f => f.Name).Value($"*{term}*")) //bad performance, use MatchPhrasePrefix
                    || new QueryContainerDescriptor<Actors>().Wildcard(w =>
                        w.Field(f => f.Description).Value($"*{term}*")) //bad performance, use MatchPhrasePrefix
                    || new QueryContainerDescriptor<Actors>().Term(w => w.Age, numero)
                    || new QueryContainerDescriptor<Actors>().Term(w => w.TotalMovies, numero);

            var result = await _actorsRepository.SearchAsync(_ => query);

            return result?.ToList();
        }

        public async Task<ActorsAggregationModel> GetActorsAggregation()
        {
            var query = new QueryContainerDescriptor<Actors>().Bool(b =>
                b.Must(m => m.Exists(e => e.Field(f => f.Description))));

            var result = await _actorsRepository.SearchAsync(_ => query, a =>
                a.Sum("TotalAge", sa => sa.Field(o => o.Age))
                    .Sum("TotalMovies", sa => sa.Field(p => p.TotalMovies))
                    .Average("AvAge", sa => sa.Field(p => p.Age)));

            var totalAge = NestExtensions.ObterBucketAggregationDouble(result.Aggregations, "TotalAge");
            var totalMovies = NestExtensions.ObterBucketAggregationDouble(result.Aggregations, "TotalMovies");
            var avAge = NestExtensions.ObterBucketAggregationDouble(result.Aggregations, "AvAge");

            return new ActorsAggregationModel { TotalAge = totalAge, TotalMovies = totalMovies, AverageAge = avAge };
        }
    }
}