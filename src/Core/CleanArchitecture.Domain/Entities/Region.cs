using System.Collections.Generic;

namespace CleanArchitecture.Domain.Entities
{
    public class Region : BaseEntity
    {
        public Region()
        {
            Territories = new HashSet<Territory>();
        }

        public string RegionDescription { get; set; }

        public ICollection<Territory> Territories { get; private set; }
    }
}