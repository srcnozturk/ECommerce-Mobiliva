using ECommerceAPI.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ECommerceAPI.API;

namespace ECommerceAPI.IntegrationTests;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : Program
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Find the DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add test database
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase("ECommerceAPI-TestDb");
            });

            // Build the service provider
            var sp = services.BuildServiceProvider();

            // Create a scope to obtain a reference to the database context
            using var scope = sp.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var context = scopedServices.GetRequiredService<ApplicationDbContext>();
            var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TProgram>>>();

            context.Database.EnsureCreated();

            try
            {
                SeedTestData(context);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Test veritabanı seed işlemi sırasında hata oluştu. Hata: {Message}", ex.Message);
                throw;
            }
        });
    }

    private void SeedTestData(ApplicationDbContext context)
    {
        // Test için örnek ürünler
        var products = new List<Core.Entities.ProductEntity>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Description = "Test Description 1",
                UnitPrice = 100,
                Category = "Test Category",
                Unit = "Adet",
                Status = true,
                CreateDate = DateTime.UtcNow
            },
            new()
            {
                Id = Guid.NewGuid(),
                Description = "Test Description 2",
                UnitPrice = 200,
                Category = "Test Category",
                Unit = "Kg",
                Status = true,
                CreateDate = DateTime.UtcNow
            }
        };

        context.Products.AddRange(products);
        context.SaveChanges();
    }
}
