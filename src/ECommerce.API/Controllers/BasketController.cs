using ECommerce.Application.CQRS.Baskts.Commands.AddItemToBasket;
using ECommerce.Application.CQRS.Baskts.Commands.CreateBaskets;
using ECommerce.Application.CQRS.Baskts.Commands.DeleteBasket;
using ECommerce.Application.CQRS.Baskts.Commands.RemoveItems;
using ECommerce.Application.CQRS.Baskts.Commands.UpdateBasket;
using ECommerce.Application.CQRS.Baskts.Queries.GetBasketByCustomerId;
using ECommerce.Application.CQRS.Baskts.Queries.GetBasketByIds;
using ECommerce.Application.DTOs.BasktDtos;
using ECommerce.Domain.Entities.Baskets.Records;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{

    public class BasketController : ApiBaseController
    {
        private readonly IMediator _mediator;
        public BasketController(IMediator mediator)
        {
            _mediator = mediator;
        }


        // GET: api/baskets/customer/5
        [HttpGet("customer/{customerId:int}")]
        public async Task<ActionResult<BasketDto>> GetByCustomerId(
            int customerId, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetBasketByCustomerIdQuery(customerId), ct);
            return result is null
                ? NotFound()
                : Ok(result);
        }

        // POST: api/baskets/items
        [HttpPost("items")]
        public async Task<IActionResult> AddItem([FromBody] AddItemToBasketRequest request, CancellationToken ct)
        {
            var command = new AddItemToBasketCommand(request.CustomerId, request.ProductId, request.Quantity);
            var result = await _mediator.Send(command, ct);
            
            return ToActionResult(result);
        }

        // DELETE: api/baskets/{basketId}/items/{productId}
        [HttpDelete("{basketId:int}/items/{productId:int}")]
        public async Task<IActionResult> RemoveItem(
            int basketId, int productId, CancellationToken ct)
        {
            var command = new RemoveItemFromBasketCommand(basketId, productId);
            var result = await _mediator.Send(command, ct);
            return ToActionResult(result);
        }

        // PUT: api/baskets/{basketId}/items/{productId}
        [HttpPut("{basketId:int}/items/{productId:int}")]
        public async Task<IActionResult> UpdateItemQuantity(
            int basketId, int productId, [FromBody] UpdateQuantityRequest request, CancellationToken ct)
        {
            var command = new UpdateItemQuantityCommand(basketId, productId, request.Quantity);
            var result = await _mediator.Send(command, ct);
            return ToActionResult(result);
        }

        // POST: api/baskets/{basketId}/coupon
        //[HttpPost("{basketId:int}/coupon")]
        //public async Task<IActionResult> ApplyCoupon(
        //    int basketId, [FromBody] ApplyCouponRequest request, CancellationToken ct)
        //{
        //    var command = new ApplyCouponCommand(basketId, request.CouponCode);
        //    var result = await _mediator.Send(command, ct);
        //    return ToActionResult(result);
        //}

        // DELETE: api/baskets/{basketId}
        [HttpDelete("{basketId:int}")]
        public async Task<IActionResult> DeleteBasket(int basketId, CancellationToken ct)
        {
            var command = new DeleteBasketCommand(basketId);
            var result = await _mediator.Send(command, ct);
            return ToActionResult(result);
        }

    }
}
