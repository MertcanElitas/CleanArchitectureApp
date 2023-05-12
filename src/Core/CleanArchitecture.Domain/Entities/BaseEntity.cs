using System;
using System.Runtime.InteropServices.JavaScript;

namespace CleanArchitecture.Domain.Entities
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public int CreatedById { get; set; }
        public int UpdatedById { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}