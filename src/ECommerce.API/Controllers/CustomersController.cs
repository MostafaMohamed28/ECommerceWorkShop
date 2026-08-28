using ECommerce.Application.Contracts.CustomerService;
using ECommerce.Application.Dtos;
using ECommerce.DAL.Entities;
using ECommerce.Domain.Contract;
using ECommerce.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Controllers;

public class CustomersController : ApiBaseController
{
    private readonly ICustomerService customerService;
    public CustomersController(ICustomerService customerService)
    {
        this.customerService = customerService;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id,CancellationToken ct)
    {
        var customer = await customerService.GetByIdAsync(id, ct);

       return ToActionResult(customer);

    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerDto dto)
    {
       
        var customer = await customerService.CreateCustomerAsync(dto);
        
        return ToActionResult(customer);
    }

    [HttpPost("{id}/upgrade-vip")]
    public async Task<IActionResult> UpgradeToVip(int id,CancellationToken ct)
    {
        var result = await customerService.UpgradeToVipAsync(id, ct);
        
        return ToActionResult(result);
    }
}
