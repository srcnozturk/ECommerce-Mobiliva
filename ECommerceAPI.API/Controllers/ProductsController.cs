using AutoMapper;
using ECommerceAPI.Application;
using ECommerceAPI.Application.Dtos;
using ECommerceAPI.Core;
using ECommerceAPI.Core.Dtos;
using ECommerceAPI.Core.Entities;
using ECommerceAPI.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
            IProductRepository productRepository,
            ICacheService cacheService,
            IMapper mapper,
            IRabbitMQService rabbitMQService,
            IOrderRepository orderRepository,
            ILogger<ProductsController> logger)
        {
            _productRepository = productRepository;
            _cacheService = cacheService;
            _mapper = mapper;
            _rabbitMQService = rabbitMQService;
            _orderRepository = orderRepository;
            _logger = logger;
        }

        [HttpGet("get")]
        public async Task<ActionResult<ApiResponse<List<ProductDto>>>> GetProducts(
          [FromQuery] string? category = null)
        {
            try
            {
                string cacheKey = $"products-{category ?? "all"}";
                _logger.LogInformation("Getting products for category: {Category}", category ?? "all");

                var cachedProducts = _cacheService.Get<List<ProductDto>>(cacheKey);
                if (cachedProducts != null)
                {
                    _logger.LogInformation("Products retrieved from cache for category: {Category}", category ?? "all");
                    return Ok(ApiResponseFactory.Success(cachedProducts));
                }

                var products = category == null
                    ? await _productRepository.GetAllAsync()
                    : await _productRepository.GetByCategoryAsync(category);

                var productDtos = _mapper.Map<List<ProductDto>>(products);

                _cacheService.Set(cacheKey, productDtos, TimeSpan.FromHours(1));
                _logger.LogInformation("Products cached for category: {Category}", category ?? "all");

                return Ok(ApiResponseFactory.Success(productDtos));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving products for category: {Category}", category ?? "all");
                return StatusCode(500, ApiResponseFactory.ProductError<List<ProductDto>>(
                    "An error occurred while retrieving products"));
            }
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Guid>>> CreateOrder(
            [FromBody] CreateOrderRequest request)
        {
            try
            {
                _logger.LogInformation("Creating order for customer: {CustomerEmail}", request.CustomerEmail);

                var order = _mapper.Map<OrderEntity>(request);
                var createdOrder = await _orderRepository.CreateOrderWithDetailsAsync(order);

                var emailMessage = new EmailMessageDto
                {
                    To = order.CustomerEmail,
                    Subject = "Your Order Confirmation",
                    Body = $"Dear {order.CustomerName}, your order #{createdOrder.Id} has been received."
                };

                await _rabbitMQService.PublishMessageAsync("SendMail", emailMessage);
                _logger.LogInformation("Order created successfully. OrderId: {OrderId}", createdOrder.Id);

                return Ok(ApiResponseFactory.Success(createdOrder.Id, "Order created successfully"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating order for customer: {CustomerEmail}", request.CustomerEmail);
                return StatusCode(500, ApiResponseFactory.OrderError<Guid>(
                    "An error occurred while processing your order"));
            }
        }
    }
}
