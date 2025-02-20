using AutoMapper;
using ECommerceAPI.Application.Dtos;
using ECommerceAPI.Core;
using ECommerceAPI.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {

        private readonly IProductRepository _productRepository;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;

        public ProductsController(
            IProductRepository productRepository,
            ICacheService cacheService,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _cacheService = cacheService;
            _mapper = mapper;
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
    }
}
