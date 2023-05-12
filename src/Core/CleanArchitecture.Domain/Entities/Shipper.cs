using System.Collections.Generic;

namespace CleanArchitecture.Domain.Entities
{
    public class Shipper : BaseEntity
    {
        public Shipper()
        {
            Orders = new HashSet<Order>();
        }

        public string CompanyName { get; set; }
        public string Phone { get; set; }

        public ICollection<Order> Orders { get; private set; }
    }
}