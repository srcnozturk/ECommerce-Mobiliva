using AutoMapper;
using ECommerceAPI.Application.Dtos;
using ECommerceAPI.Core;
using ECommerceAPI.Core.Dtos;
using ECommerceAPI.Core.Entities;
using ECommerceAPI.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ECommerceAPI.Application.Commands;

/// <summary>
/// Command to create a new order in the system
/// </summary>
/// <param name="Request">The order creation request containing customer and order details</param>
public record CreateOrderCommand(CreateOrderRequest Request) : IRequest<ApiResponse<Guid>>;

/// <summary>
/// Handler for processing order creation commands.
/// Creates a new order and sends a confirmation email via RabbitMQ.
/// </summary>
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, ApiResponse<Guid>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IRabbitMQService _rabbitMQService;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

    /// <summary>
    /// Initializes a new instance of the CreateOrderCommandHandler
    /// </summary>
    /// <param name="orderRepository">Repository for order operations</param>
    /// <param name="rabbitMQService">Service for sending messages to RabbitMQ</param>
    /// <param name="mapper">AutoMapper instance for object mapping</param>
    /// <param name="logger">Logger for the handler</param>
    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        IRabbitMQService rabbitMQService,
        IMapper mapper,
        ILogger<CreateOrderCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _rabbitMQService = rabbitMQService;
        _mapper = mapper;
        _logger = logger;
    }

    /// <summary>
    /// Handles the creation of a new order and sends a confirmation email
    /// </summary>
    /// <param name="command">The create order command containing the order details</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>API response containing the ID of the created order</returns>
    public async Task<ApiResponse<Guid>> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating order for customer: {CustomerEmail}", command.Request.CustomerEmail);

            var order = _mapper.Map<OrderEntity>(command.Request);
            var createdOrder = await _orderRepository.CreateOrderWithDetailsAsync(order);

            var emailMessage = new EmailMessageDto
            {
                To = order.CustomerEmail,
                Subject = "Your Order Confirmation",
                Body = $"Dear {order.CustomerName}, your order #{createdOrder.Id} has been received."
            };

            await _rabbitMQService.PublishMessageAsync("SendMail", emailMessage);
            _logger.LogInformation("Order created successfully. OrderId: {OrderId}", createdOrder.Id);

            return ApiResponseFactory.Success(createdOrder.Id, "Order created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating order for customer: {CustomerEmail}", command.Request.CustomerEmail);
            return ApiResponseFactory.OrderError<Guid>("An error occurred while processing your order");
        }
    }
}
