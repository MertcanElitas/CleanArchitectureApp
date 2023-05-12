using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Interfaces.Repositories
{
    public interface ICustomerRepository
    {
        Task<IEnumerable<Customer>> GetCustomers();
        Task<Customer> GetById(int id);
        Task<bool> Insert(Customer customer, bool isCommit = false);
        Task<bool> BulkInsert(List<Customer> customers, bool isCommit = false);
        Task<bool> Delete(Customer customer, bool isCommit = false);
        Task<bool> DeleteById(int id, bool isCommit = false);
        Task<bool> SaveChangeAsync();
    }
}