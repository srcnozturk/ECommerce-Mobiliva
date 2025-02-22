using AutoMapper;
using ECommerceAPI.Application.Dtos;
using ECommerceAPI.Core;
using ECommerceAPI.Core.Dtos;
using ECommerceAPI.Core.Entities;
using ECommerceAPI.Core.Interfaces;
using ECommerceAPI.Infrastructure.Repositories;
using ECommerceAPI.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;

namespace ECommerceAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ICacheService _cacheService;
        private readonly IRabbitMQService _rabbitMQService;
        private readonly IMapper _mapper;

        public ProductsController(
            IProductRepository productRepository,
            ICacheService cacheService,
            IMapper mapper,
            IRabbitMQService rabbitMQService,
            IOrderRepository orderRepository)
        {
            _productRepository = productRepository;
            _cacheService = cacheService;
            _mapper = mapper;
            _rabbitMQService = rabbitMQService;
            _orderRepository = orderRepository;
        }

        [HttpGet("get")]
        public async Task<ActionResult<ApiResponse<List<ProductDto>>>> GetProducts(
          [FromQuery] string? category = null)
        {
            try
            {
                string cacheKey = $"products-{category ?? "all"}";

                // Cache'den kontrol
                var cachedProducts = _cacheService.Get<List<ProductDto>>(cacheKey);
                if (cachedProducts != null)
                {
                    return Ok(new ApiResponse<List<ProductDto>>
                    {
                        Data = cachedProducts
                    });
                }

                // Cache boşsa repository'den al
                var products = category == null
                    ? await _productRepository.GetAllAsync()
                    : await _productRepository.GetByCategoryAsync(category);

                var productDtos = _mapper.Map<List<ProductDto>>(products);

                // Cache'e kaydet
                _cacheService.Set(cacheKey, productDtos, TimeSpan.FromHours(1));

                return Ok(new ApiResponse<List<ProductDto>>
                {
                    Data = productDtos
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<List<ProductDto>>
                {
                    Success = false,
                    Message = "An error occurred while retrieving products"
                });
            }
        }
        [HttpPost]
        public async Task<ActionResult<ApiResponse<Guid>>> CreateOrder(
            [FromBody] CreateOrderRequest request)
        {
            try
            {
                // Sipariş oluştur
                var order = _mapper.Map<OrderEntity>(request);


                // Siparişi kaydet
                var createdOrder = await _orderRepository.CreateOrderWithDetailsAsync(order);

                // Mail gönderimi için kuyruğa ekle
                var emailMessage = new EmailMessageDto
                {
                    To = order.CustomerEmail,
                    Subject = "Your Order Confirmation",
                    Body = $"Dear {order.CustomerName}, your order #{createdOrder.Id} has been received."
                };

                _rabbitMQService.PublishMessage("SendMail", emailMessage);

                return Ok(new ApiResponse<Guid>
                {
                    Status = Status.Success,
                    ResultMessage = "Order created successfully",
                    Data = createdOrder.Id
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<Guid>
                {
                    Status = Status.Failed,
                    ResultMessage = "An error occurred while creating the order",
                    ErrorCode = "ORDER_CREATE_ERROR"
                });
            }
        }
    }
}
