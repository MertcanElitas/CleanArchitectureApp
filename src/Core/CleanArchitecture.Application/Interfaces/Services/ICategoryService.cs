using System.Threading.Tasks;
using CleanArchitecture.Application.Dtos;

namespace CleanArchitecture.Application.Interfaces.Services
{
    public interface ICategoryService
    {
        Task<bool> BulkInsert();

        Task<CategorySuggestModel> Suggest(string keyword);
    }
}