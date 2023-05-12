using System.Formats.Asn1;
using CleanArchitecture.Application.Interfaces.Repositories;
using CleanArchitecture.Application.Interfaces.Services;
using CleanArchitecture.Application.Services;
using CleanArchitecture.Persistence.DataAccess.ElasticSearch;
using CleanArchitecture.Persistence.DataAccess.ElasticSearch.Repository;
using CleanArchitecture.Persistence.DataAccess.EntityFramework;
using CleanArchitecture.Persistence.DataAccess.EntityFramework.Repository;
using CleanArchitecture.Persistence.DataAccess.MongoDB;
using CleanArchitecture.Persistence.DataAccess.MongoDB.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using CustomerRepository = CleanArchitecture.Persistence.DataAccess.MongoDB.Repository.CustomerRepository;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<DatabaseSettings>(builder.Configuration.GetSection("DatabaseSettings"));

#region " Repositories "

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IActorRepository, ActorRepository>();
builder.Services.AddScoped<ICategoryElasticRepository, CategoryElasticRepository>();
builder.Services.AddScoped<IMongoInstanceHelper, MongoInstanceHelper>();
builder.Services.AddSingleton<IElasticProvider, ElasticProvider>();
//builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
//builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services
    .AddScoped<ICustomerRepository,
        CleanArchitecture.Persistence.DataAccess.ElasticSearch.Repository.CustomerRepository>();
builder.Services.AddScoped<ICustomerElasticRepository, CustomerElasticRepository>();

#endregion

#region " Services "

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IActorService, ActorService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddSingleton<IDatabaseSettings>(sp =>
{
    return sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
});

#endregion


builder.Services.AddDbContext<NorthwindDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("NorthwindDatabase"),
        sqlServerOptionsAction => sqlServerOptionsAction.MigrationsAssembly("CleanArchitecture.Api")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();