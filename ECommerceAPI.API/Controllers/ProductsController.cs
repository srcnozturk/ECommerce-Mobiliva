using ECommerceAPI.Application;
using ECommerceAPI.Application.Commands;
using ECommerceAPI.Application.Dtos;
using ECommerceAPI.Application.Queries;
using ECommerceAPI.Core;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IMediator _mediator;

        public ProductsController(
            ILogger<ProductsController> logger,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        /// Gets products by category. If category is null, returns all products.
        /// Products are retrieved from cache if available, otherwise from database.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<ProductDto>>>> GetProducts(
            [FromQuery] string? category = null)
        {
            try
            {
                var query = new GetProductsQuery(category);
                var result = await _mediator.Send(query);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving products for category: {Category}", category ?? "all");
                return StatusCode(500, ApiResponseFactory.ProductError<List<ProductDto>>(
                    "An error occurred while retrieving products"));
            }
        }

        /// <summary>
        /// Creates a new order and sends confirmation email asynchronously via RabbitMQ.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ApiResponse<Guid>>> CreateOrder(
            [FromBody] CreateOrderRequest request)
        {
            try
            {
                var command = new CreateOrderCommand(request);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning("Validation failed for order request: {Errors}", 
                    string.Join(", ", ex.Errors.Select(e => e.ErrorMessage)));
                return BadRequest(ApiResponseFactory.ValidationError<Guid>(
                    string.Join(", ", ex.Errors.Select(e => e.ErrorMessage))));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating order");
                return StatusCode(500, ApiResponseFactory.OrderError<Guid>(
                    "An error occurred while processing your order"));
            }
        }
    }
}
