using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Application.Dtos;
using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Application.Interfaces.Services;

namespace CleanArchitecture.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly  IProductRepository _productRepository;

        public ProductService (IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<ProductListModel>> GetProductList()
        {
            return _productRepository
                .GetProducts()
                .Result
                .Select(x => new ProductListModel())
                .ToList();
        }
    }
}