
using System.Collections.Generic;
using System.Threading.Tasks;
using CleanArchitecture.Application.Dtos;

namespace CleanArchitecture.Application.Interfaces.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductListModel>> GetProductList();
    }
}