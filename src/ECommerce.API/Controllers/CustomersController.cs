using ECommerce.Application.Contracts.CustomerService;
using ECommerce.Application.CQRS.Customers.Commands.CreateCustomer;
using ECommerce.Application.CQRS.Customers.Commands.UpgradeToVip;
using ECommerce.Application.CQRS.Customers.Queries.GetById;
using ECommerce.Application.Dtos;
using ECommerce.DAL.Entities;
using ECommerce.Domain.Contract;
using ECommerce.Infrastructure.Context;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Controllers;

public class CustomersController : ApiBaseController
{
    private readonly IMediator _mediator;
    public CustomersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id,CancellationToken ct)
    {
        var customer = await _mediator.Send(new GetByIdQuery(id), ct);

       return ToActionResult(customer);

    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerDto dto)
    {
       
        var customer = await _mediator.Send(new CreateCustomerCommand(dto));
        
        return ToActionResult(customer);
    }

    [HttpPost("{id}/upgrade-vip")]
    public async Task<IActionResult> UpgradeToVip(int id,CancellationToken ct)
    {
        var result = await _mediator.Send(new UpgradeToVipCommand(id), ct);
        
        return ToActionResult(result);
    }
}
