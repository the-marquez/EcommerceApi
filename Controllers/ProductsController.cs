
using AutoMapper;
using EcommerceApi.Models;
using EcommerceApi.Models.Dtos;
using EcommerceApi.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;

        public ProductsController(IProductRepository productRepository, ICategoryRepository categoryRepository, IMapper mapper)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetProducts()
        {
            var products = _productRepository.GetProducts();
            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);
            return Ok(productDtos);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetProduct(int id)
        {
            var product = _productRepository.GetProduct(id);
            
            if (product == null)
            {
                return NotFound($"Producto con id {id} no encontrado.");
            }

            var productDto = _mapper.Map<ProductDto>(product);

            return Ok(productDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateProduct([FromBody] CreateProductDto createProductDto)
        {
            if(createProductDto is null)
            {
                return BadRequest(ModelState);
            }

            if(_productRepository.ProductExists(createProductDto.Name))
            {
                ModelState.AddModelError("CustomError", "Product already exists!");
                return BadRequest(ModelState);
            }

            if(_categoryRepository.CategoryExists(createProductDto.CategoryId) == false)
            {
                ModelState.AddModelError("CustomError", "Category does not exist!");
                return BadRequest(ModelState);
            }

            var product = _mapper.Map<Product>(createProductDto);

            if (!_productRepository.CreateProduct(product))
            {
                ModelState.AddModelError("CustomError", $"Something went wrong when saving the record {product.Name}");
                return StatusCode(500, ModelState);
            }

            var createdProduct = _productRepository.GetProduct(product.Id);
            var productDto = _mapper.Map<ProductDto>(createdProduct);

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, productDto);
        }

        [HttpGet("search/category/{categoryId:int}", Name = "GetProductByCategory")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetProductByCategory(int categoryId)
        {
            var products = _productRepository.GetProductsByCategory(categoryId);
            if(products.Count == 0)
            {
                return NotFound($"No products found for category with id {categoryId}.");
            }

            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

            return Ok(productDtos);
        }

        [HttpGet("search/text/{productName}", Name = "GetProductByNameOrDescription")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult GetProductByNameOrDescription(string productName)
        {
            var products = _productRepository.SearchProducts(productName);
            if(products.Count == 0)
            {
                return NotFound($"No products found for name {productName}.");
            }

            var productDtos = _mapper.Map<IEnumerable<ProductDto>>(products);

            return Ok(productDtos);
        }

    }
}