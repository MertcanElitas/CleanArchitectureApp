using System.Collections.Generic;

namespace CleanArchitecture.Domain.Entities
{
    public class Territory : BaseEntity
    {
        public Territory()
        {
            EmployeeTerritories = new HashSet<EmployeeTerritory>();
        }

        public string TerritoryDescription { get; set; }
        public int RegionId { get; set; }

        public Region Region { get; set; }
        public ICollection<EmployeeTerritory> EmployeeTerritories { get; private set; }
    }
}