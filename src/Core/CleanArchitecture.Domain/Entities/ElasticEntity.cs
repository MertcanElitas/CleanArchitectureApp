using Nest;

namespace CleanArchitecture.Domain.Entities
{
    public class ElasticEntity:BaseEntity
    {
        public virtual CompletionField Suggest { get; set; }
        public virtual string SearchingArea { get; set; }
        public virtual double? Score { get; set; }
    }
}