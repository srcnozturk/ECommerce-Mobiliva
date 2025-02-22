using AutoMapper;
using ECommerceAPI.Application.Dtos;
using ECommerceAPI.Core;
using ECommerceAPI.Core.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ECommerceAPI.Application.Queries;

public record GetProductsQuery(string? Category) : IRequest<ApiResponse<List<ProductDto>>>;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, ApiResponse<List<ProductDto>>>
{
    private readonly IProductRepository _productRepository;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;
    private readonly ILogger<GetProductsQueryHandler> _logger;
    private const string ALL_PRODUCTS_CACHE_KEY = "all-products";

    public GetProductsQueryHandler(
        IProductRepository productRepository,
        ICacheService cacheService,
        IMapper mapper,
        ILogger<GetProductsQueryHandler> logger)
    {
        _productRepository = productRepository;
        _cacheService = cacheService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<List<ProductDto>>> Handle(GetProductsQuery query, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Attempting to retrieve products for category: {Category}", query.Category ?? "all");

            // Try to get all products from cache first
            var allProducts = _cacheService.Get<List<ProductDto>>(ALL_PRODUCTS_CACHE_KEY);
            
            // If not in cache, get all products from database and cache them
            if (allProducts == null)
            {
                _logger.LogInformation("Cache miss - Retrieving ALL products from DATABASE");
                var products = await _productRepository.GetAllAsync();
                allProducts = _mapper.Map<List<ProductDto>>(products);
                
                // Cache all products
                _cacheService.Set(ALL_PRODUCTS_CACHE_KEY, allProducts, TimeSpan.FromHours(1));
                _logger.LogInformation("Retrieved and cached {Count} products from DATABASE", allProducts.Count);
            }
            else
            {
                _logger.LogInformation("Retrieved {Count} products from CACHE", allProducts.Count);
            }

            // If no category specified, return all products
            if (string.IsNullOrEmpty(query.Category))
            {
                return ApiResponseFactory.Success(allProducts);
            }

            // Filter products by category from cache
            var filteredProducts = allProducts.Where(p => p.Category == query.Category).ToList();
            _logger.LogInformation("Filtered {FilteredCount} products for category {Category} from cached data", 
                filteredProducts.Count, query.Category);

            return ApiResponseFactory.Success(filteredProducts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieving products for category: {Category}", query.Category ?? "all");
            return ApiResponseFactory.ProductError<List<ProductDto>>(
                "An error occurred while retrieving products");
        }
    }
}
