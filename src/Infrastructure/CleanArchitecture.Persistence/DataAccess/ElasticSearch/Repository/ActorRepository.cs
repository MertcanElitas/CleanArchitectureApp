using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Persistence.DataAccess.ElasticSearch.Repository
{
    public class ActorRepository : BaseElasticRespository<Actors>, IActorRepository
    {
        public ActorRepository(IElasticProvider elasticProvider) : base(elasticProvider)
        {
        }

        public override string IndexName  => "actors_data";
    }
}