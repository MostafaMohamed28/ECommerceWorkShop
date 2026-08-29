using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using ECommerce.Application.Dtos;
using ECommerce.Application.DTOs;
using ECommerce.DAL.Entities;
using ECommerce.Domain.Contract;
using ECommerce.Domain.Contract.Orders;
using ECommerce.Domain.Contract.Products;
using ECommerce.Domain.Contract.UnitOfWork;
using MediatR;

namespace ECommerce.Application.CQRS.Orders.Commands.Checkout
{
    public class CheckoutCommandHandler:IRequestHandler<CheckoutCommand, Result<CheckoutResponseDto>>
    {
        private readonly IOrderReadRepository _orderReadRepository;
        private readonly ICustomerReadRepository _customerReadRepository;
        private readonly IProductReadRepository _productReadRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CheckoutCommandHandler(IOrderReadRepository orderReadRepository,IProductReadRepository productReadRepository, ICustomerReadRepository customerReadRepository, IUnitOfWork unitOfWork)
        {
            _orderReadRepository = orderReadRepository;
            _productReadRepository = productReadRepository;
            _customerReadRepository = customerReadRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<CheckoutResponseDto>> Handle(CheckoutCommand request, CancellationToken ct = default)
        {
            if (request.request.Items == null || !request.request.Items.Any())
                return Result<CheckoutResponseDto>.Fail(Error.Validation("Order.ItemsRequired", "At least one item is required to create an order."));

            var customer = await _customerReadRepository.GetByIdAsync(request.request.CustomerId, ct);
            if (customer == null)
                return Result<CheckoutResponseDto>.Fail(Error.NotFound("Customer.NotFound", $"Customer with ID {request.request.CustomerId} not found."));

            decimal subtotal = 0m;
            var orderItemsToSave = new List<OrderItem>();
            var productsToUpdate = new List<Product>();


            foreach (var itemDto in request.request.Items)
            {
                if (itemDto.Quantity <= 0)
                    return Result<CheckoutResponseDto>.Fail(Error.Validation("Order.InvalidQuantity", $"Quantity for product ID {itemDto.ProductId} must be greater than zero."));

                var product = await _productReadRepository.GetProductByIdAsync(itemDto.ProductId, ct);
                if (product == null)
                    return Result<CheckoutResponseDto>.Fail(Error.NotFound("Product.NotFound", $"Product with ID {itemDto.ProductId} not found."));

                if (product.StockQuantity < itemDto.Quantity)
                    return Result<CheckoutResponseDto>.Fail(Error.Validation("Order.InsufficientStock", $"Insufficient stock for product '{product.Name}'. Available: {product.StockQuantity}, Requested: {itemDto.Quantity}"));

                subtotal += product.Price * itemDto.Quantity;

                orderItemsToSave.Add(new OrderItem
                {
                    ProductId = product.Id,
                    Quantity = itemDto.Quantity,
                    UnitPrice = product.Price
                });

                product.StockQuantity -= itemDto.Quantity;
                productsToUpdate.Add(product);
            }


            decimal discount = 0m;
            if (customer.IsVip)
            {
                discount += Math.Round(subtotal * 0.15m, 2);
            }

            if (!string.IsNullOrWhiteSpace(request.request.CouponCode))
            {
                var coupon = await _orderReadRepository.GetCouponByCodeAsync(request.request.CouponCode, ct);

                if (coupon != null)
                {
                    discount += Math.Round(subtotal * (coupon.DiscountPercentage / 100m), 2);
                }
                else
                {
                    return Result<CheckoutResponseDto>.Fail(Error.Validation("Order.InvalidCoupon", $"Invalid or inactive coupon code '{request.request.CouponCode}'."));
                }
            }

            if (discount > subtotal)
            {
                discount = subtotal;
            }

            var netAmount = subtotal - discount;
            var tax = Math.Round(netAmount * 0.14m, 2);
            var shipping = netAmount >= 1000m ? 0m : 75m;
            var finalTotal = netAmount + tax + shipping;

            if (finalTotal > 50000m)
                return Result<CheckoutResponseDto>.Fail(Error.Validation("Order.ExceedsLimit", $"The final total amount of {finalTotal:C} exceeds the maximum allowed limit of 50,000."));

            var txRef = $"TX-LEGACY-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

            var order = new Order
            {
                CustomerId = customer.Id,
                CreatedAt = DateTime.UtcNow,
                Status = OrderStatus.Paid,
                Subtotal = subtotal,
                DiscountAmount = discount,
                TaxAmount = tax,
                ShippingFee = shipping,
                TotalAmount = finalTotal,
                Items = orderItemsToSave
            };

            var payment = new Payment
            {
                Order = order,
                Amount = finalTotal,
                PaymentDate = DateTime.UtcNow,
                TransactionReference = txRef,
                IsSuccess = true
            };

            using var transaction = await _unitOfWork.GetTransaction(ct);
            try
            {
                await _unitOfWork.AddAsync(order, ct);
                await _unitOfWork.AddAsync(payment, ct);
                await _unitOfWork.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return Result<CheckoutResponseDto>.Fail(Error.Failure("Order.CheckoutFailed", "An error occurred while processing the order. Please try again."));
            }

            var response = new CheckoutResponseDto
            {
                OrderId = order.Id,
                Status = order.Status.ToString(),
                Subtotal = order.Subtotal,
                Discount = order.DiscountAmount,
                Tax = order.TaxAmount,
                Shipping = order.ShippingFee,
                Total = order.TotalAmount,
                TransactionReference = txRef
            };

            return Result<CheckoutResponseDto>.Ok(response);
        }
    }
}
