using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bogus;
using CleanArchitecture.Application.Dtos;
using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Application.Interfaces.Services;
using CleanArchitecture.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace CleanArchitecture.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly  ICustomerRepository _customerRepository;
        private readonly  ICustomerElasticRepository _customerElasticRepository;

        public CustomerService (ICustomerRepository customerRepository,
            ICustomerElasticRepository customerElasticRepository)
        {
            _customerRepository = customerRepository;
            _customerElasticRepository = customerElasticRepository;
        }

        public async Task<IEnumerable<CustomerListModel>> GetCustomerList()
        {
            return _customerRepository
                .GetCustomers()
                .Result
                .Select(x => new CustomerListModel()
                {
                    CustomerId = x.CustomerId,
                    Fax = x.Fax,
                    City = x.City,
                    Country = x.Country,
                    CompanyName = x.CompanyName,
                    ContactName = x.ContactName,
                    Phone = x.Phone,
                    Region = x.Region,
                    Address = x.Address,
                    ContactTitle = x.ContactTitle,
                    PostalCode = x.PostalCode
                })
                .ToList();
        }

        public async Task<bool> InsertBulkCustomer()
        {
            var listOfCustomer = new List<Customer>();
            //var count = 5000;
            var count = 1;

            for (int i = 0; i < count; i++)
            {
                var customer = new Faker<Customer>()
                    .RuleFor(c => c.CustomerId, f => f.Random.Number(1, 1000).ToString())
                    .RuleFor(c => c.Id, f => f.Random.Number(1, 10000000))
                    .RuleFor(c => c.CompanyName, f => f.Company.CompanyName())
                    .RuleFor(c => c.ContactName, f => f.Person.FirstName)
                    .RuleFor(c => c.ContactTitle, f => f.Person.LastName)
                    .RuleFor(c => c.Address, f => f.Address.FullAddress())
                    .RuleFor(c => c.City, f => f.Address.City())
                    .RuleFor(c => c.Region, f => f.Address.StreetName())
                    .RuleFor(c => c.PostalCode, f => f.Address.CountryCode())
                    .RuleFor(c => c.Country, f => f.Address.Country())
                    .RuleFor(c => c.Phone, f => f.Phone.ToString())
                    .RuleFor(c => c.Fax, f => f.Lorem.Lines())
                    .Generate();

                listOfCustomer.Add(customer);
            }

            listOfCustomer.First().City = "Çanakkale";
            listOfCustomer.First().CompanyName = "Tuğra";
            listOfCustomer.First().ContactName = "Şakir Ağaça";
            listOfCustomer.First().ContactTitle = "İsmail Çamüzen";

            return await _customerRepository.BulkInsert(listOfCustomer, true);
        }



        public async Task<bool> InsertBulkCustomerIndex()
        {
            var listOfCustomer = new List<CustomerElasticIndex>();
            //var count = 5000;
            var count = 5;

            for (int i = 0; i < count; i++)
            {
                var customer = new Faker<CustomerElasticIndex>()
                    .RuleFor(c => c.CustomerId, f => f.Random.Number(1, 1000).ToString())
                    .RuleFor(c => c.Id, f => f.Random.Number(1, 100))
                    .RuleFor(c => c.UserId, f => f.Random.Number(1, 90))
                    .RuleFor(c => c.CategoryId, f => f.Random.Number(1, 90))
                    .RuleFor(c => c.CategoryName, f => f.Commerce.ProductName())
                    .RuleFor(c => c.CompanyName, f => f.Company.CompanyName())
                    .RuleFor(c => c.ContactName, f => f.Person.FirstName)
                    .RuleFor(c => c.ContactTitle, f => f.Person.LastName)
                    .RuleFor(c => c.Address, f => f.Address.FullAddress())
                    .RuleFor(c => c.City, f => f.Address.City())
                    .RuleFor(c => c.Region, f => f.Address.StreetName())
                    .RuleFor(c => c.PostalCode, f => f.Address.CountryCode())
                    .RuleFor(c => c.Country, f => f.Address.Country())
                    .RuleFor(c => c.Phone, f => f.Phone.ToString())
                    .RuleFor(c => c.Fax, f => f.Lorem.Paragraph(5))
                    .RuleFor(c => c.SearchingArea, f => f.Lorem.Paragraph(50))
                    .Generate();

                listOfCustomer.Add(customer);
            }

            listOfCustomer.First().Suggest = new Nest.CompletionField
            {
                Input = new List<string>()
                {
                    "ElasticSearch",
                    "Full text search",
                    "search with suggestion",
                    "suggestion",
                    "speed search",
                    "nosql search"
                }
            };


            return await _customerElasticRepository.BulkInsertCustomerElastic(listOfCustomer, true);
        }

        public async Task<List<CustomerElasticIndex>> SearchWithTerm(string searchText)
        {
            var data = await _customerElasticRepository.GetSearchAsyncWithTerm(searchText);

            return data;
        }

        public async Task<List<CustomerElasticIndex>> GetSearchAsyncWithTerms(int userId, int categoryId,
            string categoryName, string contactName)
        {
            var data = await _customerElasticRepository.GetSearchAsyncWithTerms(userId, categoryId, categoryName,
                contactName);

            return data;
        }


        public async Task<List<CustomerElasticIndex>> GetSearchAsyncWithTermsIds(List<int> userIds)
        {
            var data = await _customerElasticRepository.GetSearchAsyncWithTermsIds(userIds);

            return data;
        }

        public async Task<List<CustomerElasticIndex>> GetSearchAsyncWithTermsIds(List<int> userIds,
            List<int> categoryIds)
        {
            var data = await _customerElasticRepository.GetSearchAsyncWithTermsMultipleIds(userIds, categoryIds);

            return data;
        }

        public async Task<List<CustomerElasticIndex>> SuggestSearchAsync(string searchText)
        {
            var data = await _customerElasticRepository.SuggestSearchAsync(searchText);

            return data;
        }

        public async Task<List<CustomerElasticIndex>> GetSearchAsyncWithTermsByString(string country,
            string contactTitle)
        {
            var data = await _customerElasticRepository.GetSearchAsyncWithTermsByString(country, contactTitle);

            return data;
        }

        public async Task<List<CustomerElasticIndex>> GetSearchAsyncWithTermsMultipleIdsV2(List<int> userIds,
            List<int> categoryIds)
        {
            var data = await _customerElasticRepository.GetSearchAsyncWithTermsMultipleIdsV2(userIds, categoryIds);

            return data;
        }
    }
}