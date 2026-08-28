using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ECommerce.Application.Common;
using ECommerce.Application.Contracts.OrderService;
using ECommerce.Application.Contracts.ProductService;
using ECommerce.Application.Dtos;
using ECommerce.Application.DTOs;
using ECommerce.DAL.Entities;
using ECommerce.Domain.Contract;
using ECommerce.Domain.Contract.Orders;
using ECommerce.Domain.Contract.Products;
using ECommerce.Domain.Contract.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Application.Services.OrderServices
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICustomerRepository _customerRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductRepository _productRepository;
        public OrderService(IOrderRepository orderRepository, ICustomerRepository customerRepository, IUnitOfWork unitOfWork, IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _customerRepository = customerRepository;
            _unitOfWork = unitOfWork;   
            _productRepository = productRepository;
        }

        public async Task<Result> CancelOrderAsync(int id, CancellationToken ct = default)
        {
           var order=await _orderRepository.GetOrderByIdAsync(id, ct);
            if (order == null)
            {
                return Result.Fail(Error.NotFound("Order.NotFound", $"Order with ID {id} not found."));
            }
            if (order.Status == OrderStatus.Cancelled)
            {
                return Result.Fail(Error.Conflict("Order.AlreadyCancelled", $"Order with ID {id} is already cancelled."));
            }

            if(order.Status == OrderStatus.Paid)
            {
                foreach (var item in order.Items)
                {
                    var product = item.Product;
                    if (product is not null)
                    {
                        product.StockQuantity += item.Quantity;
                    }
                }
            }
            order.Status = OrderStatus.Cancelled;
            await _unitOfWork.SaveChangesAsync(ct);

            return Result.Ok();
        }

        public async Task<Result<Order>> GetOrderByIdAsync(int id, CancellationToken ct = default)
        {
            var order= await _orderRepository.GetOrderByIdAsync(id, ct);
            if (order == null)
            {
                return Result<Order>.Fail(Error.NotFound("OrderNotFound", $"Order with ID {id} not found."));
            }
            return Result<Order>.Ok(order);
        }

        public async Task<Result<IReadOnlyList<Order>>> GetOrdersByCustomerIdAsync(int customerId, CancellationToken ct = default)
        {
            var customerExists = await _customerRepository.ExistsAsync(customerId, ct);
            if (!customerExists)
                return Result<IReadOnlyList<Order>>.Fail(Error.NotFound("Customer.NotFound", $"Customer with ID {customerId} not found."));
            
            var orders = await _orderRepository.GetByCustomerIdAsync(customerId, ct);
            return Result<IReadOnlyList<Order>>.Ok(orders);
        }
        public async Task<Result<CheckoutResponseDto>> CheckoutAsync(CreateOrderDto request, CancellationToken ct = default)
        {
            if (request.Items == null || !request.Items.Any())
                return  Result<CheckoutResponseDto>.Fail(Error.Validation("Order.ItemsRequired", "At least one item is required to create an order."));

            var customer = await _customerRepository.GetByIdAsync(request.CustomerId, ct);
            if (customer == null)
                return Result<CheckoutResponseDto>.Fail(Error.NotFound("Customer.NotFound", $"Customer with ID {request.CustomerId} not found."));

            decimal subtotal = 0m;
            var orderItemsToSave = new List<OrderItem>();
            var productsToUpdate = new List<Product>();


            foreach (var itemDto in request.Items)
            {
                if (itemDto.Quantity <= 0)
                    return Result<CheckoutResponseDto>.Fail(Error.Validation("Order.InvalidQuantity", $"Quantity for product ID {itemDto.ProductId} must be greater than zero."));

                var product = await _productRepository.GetProductByIdAsync(itemDto.ProductId, ct);
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

            if (!string.IsNullOrWhiteSpace(request.CouponCode))
            {
                var coupon =await _orderRepository.GetCouponByCodeAsync(request.CouponCode, ct);

                if (coupon != null)
                {
                    discount += Math.Round(subtotal * (coupon.DiscountPercentage / 100m), 2);
                }
                else
                {
                    return Result<CheckoutResponseDto>.Fail(Error.Validation("Order.InvalidCoupon", $"Invalid or inactive coupon code '{request.CouponCode}'."));
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

            using var transaction =await _unitOfWork.GetTransaction(ct);
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
