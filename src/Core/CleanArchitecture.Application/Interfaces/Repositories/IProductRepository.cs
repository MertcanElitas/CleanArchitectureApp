using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Interfaces.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetProducts();
        Task<Product> GetById(int id);
        Task<bool> Insert(Product product, bool isCommit = false);
        Task<bool> Delete(Product product, bool isCommit = false);
        Task<bool> DeleteById(int id, bool isCommit = false);
        Task<bool> SaveChangeAsync();
    }
}