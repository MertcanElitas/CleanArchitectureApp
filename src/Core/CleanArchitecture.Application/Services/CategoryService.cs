using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Application.Dtos;
using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Application.Interfaces.Services;
using CleanArchitecture.Domain.Entities;
using Nest;

namespace CleanArchitecture.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryElasticRepository _elasticRepository;

        public CategoryService (ICategoryElasticRepository elasticRepository)
        {
            _elasticRepository = elasticRepository;
        }

        public Task<bool> BulkInsert()
        {
            List<Category> categories = new List<Category>();

            categories.Add(new Category()
            {
                Id = 1,
                Name = "Samsung Galaxy Note 8",
                Suggest = new CompletionField()
                {
                    Input = new [] { "Samsung Galaxy Note 8", "Galaxy Note 8", "Note 8" }
                }
            });

            categories.Add(new Category()
            {
                Id = 2,
                Name = "Samsung Galaxy S8",
                Suggest = new CompletionField()
                {
                    Input = new[] { "Samsung Galaxy S8", "Galaxy S8", "S8" }
                }
            });

            categories.Add(new Category()
            {
                Id = 3,
                Name = "Apple Iphone 8",
                Suggest = new CompletionField()
                {
                    Input = new[] { "Apple Iphone 8", "Iphone 8" }
                }
            });

            categories.Add(new Category()
            {
                Id = 4,
                Name = "Apple Iphone X",
                Suggest = new CompletionField()
                {
                    Input = new[] { "Apple Iphone X", "Iphone X" }
                }
            });

            categories.Add(new Category()
            {
                Id = 5,
                Name = "Apple iPad Pro",
                Suggest = new CompletionField()
                {
                    Input = new[] { "Apple iPad Pro", "iPad Pro" }
                }
            });

            bool isCreated = _elasticRepository.CreateIndexAsync("product_suggest").Result;

            if (isCreated)
            {
                _elasticRepository.IndexAsync("product_suggest", categories).Wait();

                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public async Task<CategorySuggestModel> Suggest(string keyword)
        {
            var result = new CategorySuggestModel();

            var model = await _elasticRepository.SuggestAsync("product_suggest", keyword);

            if (model.Suggests != null && model.Suggests.Any())
            {
                result.Suggests = model.Suggests.Select(x => new CategoryModel()
                {
                    Name = x.Name,
                    Score = x.Score,
                    SuggestedName = x.SuggestedName,
                    Id = x.Id
                }).ToList();
            }

            return result;
        }
    }
}