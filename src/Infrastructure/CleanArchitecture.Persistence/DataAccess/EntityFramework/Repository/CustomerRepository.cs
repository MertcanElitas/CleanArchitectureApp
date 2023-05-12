using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Persistence.DataAccess.EntityFramework.Repository
{
    public class CustomerRepository: ICustomerRepository
    {
        private readonly NorthwindDbContext _dbContext;

        public CustomerRepository (NorthwindDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Customer>> GetCustomers()
        {
            return await _dbContext.Customers.ToListAsync();
        }

        public async Task<Customer> GetById(int id)
        {
            return await _dbContext.Customers.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> Insert(Customer customer, bool isCommit = false)
        {
            _dbContext.Customers.Add(customer);

            return isCommit ? await SaveChangeAsync() : true;
        }

        public async Task<bool> BulkInsert(List<Customer> customers, bool isCommit = false)
        {
            _dbContext.Customers.AddRange(customers);

            return isCommit ? await SaveChangeAsync() : true;
        }

        public Task<bool> BulkInsertCustomerElastic(List<CustomerElasticIndex> customers, bool isCommit = false)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Delete(Customer customer, bool isCommit = false)
        {
            _dbContext.Customers.Remove(customer);

            return isCommit ? await SaveChangeAsync() : true;
        }

        public async Task<bool> DeleteById(int id, bool isCommit = false)
        {
            _dbContext.Customers.Remove(_dbContext.Customers.FirstOrDefault(x => x.Id == id));

            return isCommit ? await SaveChangeAsync() : true;
        }

        public async Task<bool> SaveChangeAsync()
        {
            try
            {
                return await _dbContext.SaveChangesAsync() > default(int);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return false;
        }
    }
}