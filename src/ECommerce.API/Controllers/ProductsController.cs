using ECommerce.Application.Contracts.ProductService;
using ECommerce.Application.Dtos;
using ECommerce.Application.DTOs;
using ECommerce.DAL.Entities;
using ECommerce.Infrastructure.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Controllers;

public class ProductsController : ApiBaseController
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var products = await _productService.GetAllProductsAsync(ct);

        return ToActionResult(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id,CancellationToken ct)
    {
        var productResult = await _productService.GetProductByIdAsync(id, ct);
      
        return ToActionResult(productResult);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto, CancellationToken ct)
    {
        var result = await _productService.CreateProductAsync(dto, ct);
        
        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.data);

        return ToActionResult(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductDto dto, CancellationToken ct)
    {
        var result = await _productService.UpdateProductAsync(id, dto, ct);

        return ToActionResult(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id,CancellationToken ct)
    {
        var result = await _productService.DeleteProductAsync(id, ct);

        return ToActionResult(result);
    }
}
