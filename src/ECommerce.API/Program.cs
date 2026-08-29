using ECommerce.Application.CQRS.Customers.Queries.GetById;
using ECommerce.Application.CQRS.Orders.Queries.GetOrdersByCustomerId;
using ECommerce.Application.CQRS.Products.Queries.GetAllProduct;
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
builder.Services.AddScoped<ICustomerReadRepository, CustomerReadRepository>();
builder.Services.AddScoped<ICustomerWriteRepository, CustomerWriteRepository>();
builder.Services.AddScoped<IOrderReadRepository, OrderReadRepository>();
builder.Services.AddScoped<IProductReadRepository, ProductReadRepository>();
builder.Services.AddScoped<IProductWriteRepository, ProductWriteRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(GetAllProductsQueryHandler).Assembly));

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(GetOrdersByCustomerIdQueryHandler).Assembly));

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(GetByIdQueryHandler).Assembly));

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
