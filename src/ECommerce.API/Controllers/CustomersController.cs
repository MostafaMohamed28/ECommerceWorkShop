using ECommerce.API.DTOs;
using ECommerce.Application.Contracts;
using ECommerce.Application.Dtos;
using ECommerce.DAL.Context;
using ECommerce.DAL.Entities;
using ECommerce.Domain.Contract;
using ECommerce.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private readonly ICustomerService customerService;
    public CustomersController(ICustomerService customerService)
    {
        this.customerService = customerService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Customer>> GetById(int id,CancellationToken ct)
    {
        var customer = await customerService.GetByIdAsync(id, ct);

        if (customer is null)
        {
            return NotFound($"Customer with ID {id} not found.");
        }

        return new OkObjectResult(customer);

    }

    [HttpPost]
    public async Task<ActionResult<Customer>> Create([FromBody] CreateCustomerDto dto)
    {
       
        var customer = await customerService.CreateCustomerAsync(dto);
        if (customer is null)
        {
          return BadRequest("Failed to create customer.");
        }
        return CreatedAtAction(nameof(GetById), new { id = customer.data.Id }, customer);
    }

    [HttpPost("{id}/upgrade-vip")]
    public async Task<IActionResult> UpgradeToVip(int id,CancellationToken ct)
    {
       

        var result = await customerService.UpgradeToVipAsync(id, ct);
        if (result.IsSuccess)
        {
            return Ok(new { message = "Customer upgraded to VIP successfully." });
        }
        else
        {
            return BadRequest(result.Error);
        }
    }
}
