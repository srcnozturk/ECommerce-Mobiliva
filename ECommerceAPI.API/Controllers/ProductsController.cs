using ECommerceAPI.Application;
using ECommerceAPI.Application.Dtos;
using ECommerceAPI.Core;
using FluentValidation;

namespace ECommerceAPI.API.Controllers
{
    /// <summary>
    /// API controller for managing products and orders in the e-commerce system.
    /// Provides endpoints for retrieving products and creating orders.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IMediator _mediator;

        /// <summary>
        /// Initializes a new instance of the ProductsController
        /// </summary>
        /// <param name="logger">Logger for the controller</param>
        /// <param name="mediator">Mediator for handling commands and queries</param>
        public ProductsController(
            ILogger<ProductsController> logger,
            IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        /// Retrieves products, optionally filtered by category.
        /// Products are retrieved from cache if available, otherwise from database.
        /// </summary>
        /// <param name="category">Optional category to filter products by</param>
        /// <returns>List of products matching the specified criteria</returns>
        /// <response code="200">Returns the list of products</response>
        /// <response code="500">If there was an internal error processing the request</response>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<List<ProductDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<List<ProductDto>>), StatusCodes.Status500InternalServerError)]
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
        /// <param name="request">The order creation request containing order details</param>
        /// <returns>The ID of the created order</returns>
        /// <response code="200">Returns the created order ID</response>
        /// <response code="400">If the request validation fails</response>
        /// <response code="500">If there was an internal error processing the order</response>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<Guid>), StatusCodes.Status500InternalServerError)]
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
