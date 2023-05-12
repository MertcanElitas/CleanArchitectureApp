using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Persistence.DataAccess.MongoDB.Helpers;
using MongoDB.Driver;

namespace CleanArchitecture.Persistence.DataAccess.MongoDB.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IMongoCollection<Customer> _mongoCustomerCollection;
        private readonly IMongoInstanceHelper mongoDatabase;

        public CustomerRepository(IMongoInstanceHelper _mongoDatabase, IDatabaseSettings databaseSettings)
        {
            mongoDatabase = _mongoDatabase;
            _mongoCustomerCollection =
                _mongoDatabase.GetMongoCollectionByName<Customer>(databaseSettings.CourseCollectionName);
        }

        public async Task<IEnumerable<Customer>> GetCustomers()
        {
            var customers = await _mongoCustomerCollection
                .Find(x => true)
                .ToListAsync();

            return customers;
        }

        public async Task<Customer> GetById(int id)
        {
            var customers = await _mongoCustomerCollection
                .Find(x => x.Id == id)
                .FirstAsync();

            return customers;
        }

        public async Task<bool> Insert(Customer customer, bool isCommit = false)
        {
            await _mongoCustomerCollection.InsertOneAsync(customer);

            return true;
        }

        public async Task<bool> BulkInsert(List<Customer> customers, bool isCommit = false)
        {
            //var session = mongoDatabase.mongoClient.StartSession();
            await _mongoCustomerCollection.InsertManyAsync(customers);

            return true;
            // session.StartTransaction();
            //
            // try
            // {
            //     session.CommitTransaction();
            // }
            // catch (Exception e)
            // {
            //     Console.WriteLine("Error writing to MongoDB: " + e.Message);
            //     session.AbortTransaction();
            // }
            //
            // return true;
        }

        public Task<bool> BulkInsertCustomerElastic(List<CustomerElasticIndex> customers, bool isCommit = false)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Delete(Customer customer, bool isCommit = false)
        {
            var deleteResult = await _mongoCustomerCollection.DeleteOneAsync(x => x.Id == customer.Id);

            if (deleteResult.DeletedCount > 0)
                return true;
            else
                return false;
        }

        public async Task<bool> DeleteById(int id, bool isCommit = false)
        {
            var deleteResult = await _mongoCustomerCollection.DeleteOneAsync(x => x.Id == id);

            if (deleteResult.DeletedCount > 0)
                return true;
            else
                return false;
        }

        public async Task<bool> SaveChangeAsync()
        {
            return true;
        }
    }
}