using AutoMapper;
using ECommerceAPI.Application.Dtos;
using ECommerceAPI.Core;
using ECommerceAPI.Core.Dtos;
using ECommerceAPI.Core.Entities;
using ECommerceAPI.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ECommerceAPI.Application.Commands;

public record CreateOrderCommand(CreateOrderRequest Request) : IRequest<ApiResponse<Guid>>;

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, ApiResponse<Guid>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IRabbitMQService _rabbitMQService;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateOrderCommandHandler> _logger;

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
