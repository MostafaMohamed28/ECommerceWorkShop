using ECommerce.Application.Common;
using ECommerce.Application.Contracts.OrderService;
using ECommerce.Application.Dtos;
using ECommerce.DAL.Entities;
using ECommerce.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Controllers;


public class OrdersController : ApiBaseController
{
    private readonly IOrderService _orderService;

    public OrdersController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Order>> GetOrder(int id,CancellationToken ct = default)
    {
        var result = await _orderService.GetOrderByIdAsync(id, ct);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }


        return NotFound(result.Error);
    }

    [HttpGet("customer/{customerId}")]
    public async Task<ActionResult<IReadOnlyList<Order>>> GetCustomerOrders(int customerId, CancellationToken ct = default)
    {
        var result = await _orderService.GetOrdersByCustomerIdAsync(customerId, ct);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return NotFound(result.Error);
    }

    [HttpPost("cancel/{id}")]
    public async Task<IActionResult> CancelOrder(int id,CancellationToken ct)
    {
        var result=await _orderService.CancelOrderAsync(id,ct);

        return ToActionResult(result);
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CreateOrderDto request, CancellationToken ct = default)
    {
        var result = await _orderService.CheckoutAsync(request, ct);
        return ToActionResult(result);
    }
}
