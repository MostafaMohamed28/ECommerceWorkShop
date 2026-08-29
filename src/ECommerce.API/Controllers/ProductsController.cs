using ECommerce.Application.Contracts.ProductService;
using ECommerce.Application.CQRS.Products.Commands.CreateProduct;
using ECommerce.Application.CQRS.Products.Commands.DeleteProduct;
using ECommerce.Application.CQRS.Products.Commands.UpdateProduct;
using ECommerce.Application.CQRS.Products.Queries.GetAllProduct;
using ECommerce.Application.CQRS.Products.Queries.GetProductByIds;
using ECommerce.Application.Dtos;
using ECommerce.Application.DTOs;
using ECommerce.DAL.Entities;
using ECommerce.Infrastructure.Context;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Controllers;

public class ProductsController : ApiBaseController
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var products = await _mediator.Send(new GetAllProductsQuery(), ct);

        return ToActionResult(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id,CancellationToken ct)
    {
        var productResult = await _mediator.Send(new GetProductByIdQuery(id), ct);
      
        return ToActionResult(productResult);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateProductCommand(dto.Name,dto.SKU,dto.Price,dto.StockQuantity), ct);
        
        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.data);

        return ToActionResult(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateProductCommand(id, dto.Name, dto.SKU, dto.Price, dto.StockQuantity), ct);

        return ToActionResult(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id,CancellationToken ct)
    {
        var result = await _mediator.Send(new DeleteProductCommand(id), ct);

        return ToActionResult(result);
    }
}
