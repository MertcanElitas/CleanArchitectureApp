using System.Collections.Generic;

namespace CleanArchitecture.Application.Dtos
{
    public class CategorySuggestModel
    {
        public IEnumerable<CategoryModel> Suggests { get; set; }
    }
    
    public class CategoryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SuggestedName { get; set; }
        public double Score { get; set; }
    }
}