using System;

namespace CleanArchitecture.Domain.Entities
{
    public class Actors : BaseElasticEntity
    {
        public DateTime RegistrationDate { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime BirthDate { get; set; }
        public int Age { get; set; }
        public int TotalMovies { get; set; }
        public string Movies { get; set; }
    }
    public class ActorsAggregationModel
    {
        public double TotalAge { get; set; }
        public double TotalMovies { get; set; }
        public double AverageAge { get; set; }
    }

    public class BaseElasticEntity
    {
        public string Id { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}