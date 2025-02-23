using ECommerceAPI.API.Middleware;
using ECommerceAPI.Application.Mapping;
using ECommerceAPI.Core.Interfaces;
using ECommerceAPI.Infrastructure;
using ECommerceAPI.Infrastructure.BackgroundServices;
using ECommerceAPI.Infrastructure.Interfaces;
using ECommerceAPI.Infrastructure.Repositories;
using ECommerceAPI.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Serilog;
using Serilog.Events;
using MediatR;
using MediatR.Pipeline;
using FluentValidation;
using ECommerceAPI.Application.Behaviors;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ECommerceAPI.API;

public class Program 
{ 
    public static WebApplication CreateWebApplication(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("logs/ecommerce-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        builder.Host.UseSerilog();

        var connectionString = builder.Configuration.GetConnectionString("Mobiliva-DB");

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
        );
        builder.Services.AddAutoMapper(typeof(ProductMappingProfile).Assembly);
        builder.Services.AddSingleton<IConnectionMultiplexer>(opt =>
            ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("Redis")));

        // Add MediatR
        builder.Services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(ProductMappingProfile).Assembly);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        });

        // Add FluentValidation
        builder.Services.AddValidatorsFromAssembly(typeof(ProductMappingProfile).Assembly);

        builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
        builder.Services.AddSingleton<IRabbitMQService, RabbitMQService>();
        builder.Services.AddScoped<IEmailService, EmailService>();

        // Background service
        builder.Services.AddHostedService<MailSenderBackgroundService>();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddControllers();
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped<IOrderRepository, OrderRepository>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddMemoryCache();
        builder.Services.AddScoped<ICacheService, MemoryCacheService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseRouting(); // Ensure routing is added
        app.UseAuthorization();

        app.UseMiddleware<ExceptionMiddleware>();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers(); // Ensure endpoints are mapped
        });

        return app;
    }

    public static void Main(string[] args)
    {
        try
        {
            Log.Information("Starting web application");
            var app = CreateWebApplication(args);
            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
