namespace CleanArchitecture.Domain.Entities
{
    public class CustomerElasticIndex : ElasticEntity
    {
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public string CustomerId { get; set; }
        public string Title { get; set; }
        public string CategoryName { get; set; }
        public string UserInfo { get; set; }
        public string CompanyName { get; set; }
        public string Url { get; set; }
        public string ContactName { get; set; }
        public string ContactTitle { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
    }
}