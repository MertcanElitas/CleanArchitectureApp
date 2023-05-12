using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Persistence.DataAccess.EntityFramework.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly NorthwindDbContext _dbContext;

        public ProductRepository (NorthwindDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _dbContext.Products.ToListAsync();
        }

        public async Task<Product> GetById(int id)
        {
            return await _dbContext.Products.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> Insert(Product product, bool isCommit = false)
        {
            _dbContext.Products.Add(product);

            return isCommit ? await SaveChangeAsync() : true;
        }

        public async Task<bool> Delete(Product product, bool isCommit = false)
        {
            _dbContext.Products.Remove(product);

            return isCommit ? await SaveChangeAsync() : true;
        }

        public async Task<bool> DeleteById(int id, bool isCommit = false)
        {
            _dbContext.Products.Remove(_dbContext.Products.FirstOrDefault(x => x.Id == id));

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