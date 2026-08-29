using ECommerce.Application.Common;
using ECommerce.Application.Contracts.OrderService;
using ECommerce.Application.CQRS.Orders.Commands.CancelOrder;
using ECommerce.Application.CQRS.Orders.Commands.Checkout;
using ECommerce.Application.CQRS.Orders.Queries.GetOrderById;
using ECommerce.Application.CQRS.Orders.Queries.GetOrdersByCustomerId;
using ECommerce.Application.Dtos;
using ECommerce.DAL.Entities;
using ECommerce.Infrastructure.Context;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Controllers;


public class OrdersController : ApiBaseController
{
    private readonly IMediator _mediator;

    public OrdersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetOrder(int id,CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetOrderByIdQuery(id), ct);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }


        return NotFound(result.Error);
    }

    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<IReadOnlyList<Order>>> GetCustomerOrders(int customerId, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetOrdersByCustomerIdQuery(customerId), ct);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return NotFound(result.Error);
    }

    [HttpPost("cancel/{id}")]
    public async Task<IActionResult> CancelOrder(int id,CancellationToken ct)
    {
        var result=await _mediator.Send(new CancelOrderCommand(id), ct);

        return ToActionResult(result);
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CreateOrderDto request, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new CheckoutCommand(request), ct);
        return ToActionResult(result);
    }
}
