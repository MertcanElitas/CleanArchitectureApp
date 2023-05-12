using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Interfaces.Repositories
{
    public interface IActorRepository : IBaseElasticRepository<Actors>
    {
    }
}