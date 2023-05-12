using System.Collections.Generic;
using Nest;

namespace CleanArchitecture.Domain.Entities
{
    public class Category : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CompletionField Suggest { get; set; }
    }

    public class CategorySuggestResponse
    {
        public IEnumerable<CategorySuggest> Suggests { get; set; }
    }

    public class CategorySuggest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SuggestedName { get; set; }
        public double Score { get; set; }
    }
}