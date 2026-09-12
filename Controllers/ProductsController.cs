
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

        [HttpPatch("buy/{productName}/quantity/{quantity:int}", Name = "BuyProduct")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult BuyProduct(string productName, int quantity)
        {
            if(string.IsNullOrEmpty(productName) || quantity <= 0)
            {
                return BadRequest("Product name and quantity must be provided.");
            }

            bool productExists = _productRepository.ProductExists(productName);

            if (!productExists)
            {
                return NotFound($"Product with name {productName} not found.");
            }

            if( !_productRepository.BuyProduct(productName, quantity))
            {
                return StatusCode(500, "An error occurred while processing the purchase.");
            }

            var units = quantity == 1 ? "unit" : "units";

            return Ok($"Successfully purchased {quantity} {units} of {productName}.");
        }



        [HttpPost("bulk")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateProductsBulk([FromBody] IEnumerable<CreateProductDto> createProductDtos)
        {
            
            if (createProductDtos is null || !createProductDtos.Any())
            {
                ModelState.AddModelError("CustomError", "The product collection cannot be empty.");
                return BadRequest(ModelState);
            }

            var validProducts = new List<Product>();

            foreach (var dto in createProductDtos)
            {
                if (_productRepository.ProductExists(dto.Name))
                {
                    ModelState.AddModelError("CustomError", $"Product '{dto.Name}' already exists.");
                    return BadRequest(ModelState);
                }

                if (!_categoryRepository.CategoryExists(dto.CategoryId))
                {
                    ModelState.AddModelError("CustomError", $"Category with id {dto.CategoryId} does not exist for product '{dto.Name}'.");
                    return BadRequest(ModelState);
                }

                var product = _mapper.Map<Product>(dto);
                validProducts.Add(product);
            }

            if (!_productRepository.CreateProducts(validProducts))
            {
                ModelState.AddModelError("CustomError", "Something went wrong when saving the records.");
                return StatusCode(500, ModelState);
            }

            var createdProductDtos = _mapper.Map<IEnumerable<ProductDto>>(validProducts);
            return CreatedAtAction(nameof(GetProducts), createdProductDtos);
        }

    }
}