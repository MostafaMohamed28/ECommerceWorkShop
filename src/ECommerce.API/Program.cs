using ECommerce.Application.Contracts.CustomerService;
using ECommerce.Application.Contracts.OrderService;
using ECommerce.Application.Contracts.ProductService;
using ECommerce.Application.Services.CustomerService;
using ECommerce.Application.Services.OrderServices;
using ECommerce.Application.Services.ProductServices;
using ECommerce.Domain.Contract;
using ECommerce.Domain.Contract.Orders;
using ECommerce.Domain.Contract.Products;
using ECommerce.Domain.Contract.UnitOfWork;
using ECommerce.Infrastructure.Context;
using ECommerce.Infrastructure.Repository.CustomerRepo;
using ECommerce.Infrastructure.Repository.OrdersRepo;
using ECommerce.Infrastructure.Repository.Products;
using ECommerce.Infrastructure.Repository.UnitOfWorkReo;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
