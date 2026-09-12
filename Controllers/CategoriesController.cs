
using AutoMapper;
using EcommerceApi.Models;
using EcommerceApi.Models.Dtos;
using EcommerceApi.Repositories.Contracts;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        public CategoriesController(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCategories()
        {
            var categories = _categoryRepository.GetCategories();
            var categoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);
            return Ok(categoryDtos);
        }

        [HttpGet("{id:int}", Name = "GetCategory")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetCategoryById(int id)
        {
            var category = _categoryRepository.GetCategoryById(id);
            if (category is null)
            {
                return NotFound($"Category with id {id} not found.");

            }
            var categoryDto = _mapper.Map<CategoryDto>(category);
            return Ok(categoryDto);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateCategory(CreateCategoryDto createCategoryDto)
        {
            if (createCategoryDto is null)
            {
                return BadRequest(ModelState);
            }

            if (_categoryRepository.CategoryExists(createCategoryDto.Name))
            {
                ModelState.AddModelError("CustomError", "Category already exists!");
                return BadRequest(ModelState);
            }

            var category = _mapper.Map<Category>(createCategoryDto);

            if (!_categoryRepository.CreateCategory(category))
            {
                ModelState.AddModelError("CustomError", $"Something went wrong when saving the record {category.Name}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetCategory", new { id = category.Id }, category);
        }

        [HttpPatch("{id:int}", Name = "UpdateCategory")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult UpdateCategory(int id, UpdateCategoryDto updateCategoryDto)
        {
            if (updateCategoryDto is null)
            {
                return BadRequest(ModelState);
            }

            if(!_categoryRepository.CategoryExists(id))
            {
                return NotFound($"Category with id {id} not found.");
            }

            if (_categoryRepository.CategoryExists(updateCategoryDto.Name))
            {
                ModelState.AddModelError("CustomError", "Category already exists!");
                return BadRequest(ModelState);
            }

            var category = _mapper.Map<Category>(updateCategoryDto);
            category.Id = id;

            if (!_categoryRepository.UpdateCategory(category))
            {
                ModelState.AddModelError("CustomError", $"Something went wrong when updating the record {category.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        
        }

        [HttpDelete("{id:int}", Name = "DeleteCategory")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteCategory(int id)
        {
            var category = _categoryRepository.GetCategoryById(id);

            if (category is null)
            {
                return NotFound($"Category with id {id} not found.");
            }

            if (!_categoryRepository.DeleteCategory(category))
            {
                ModelState.AddModelError("CustomError", $"Something went wrong when deleting the record {category.Name}");
                return StatusCode(500, ModelState);
            }

            return NoContent();
        }

        [HttpPost("bulk")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult CreateCategoriesBulk([FromBody] IEnumerable<CreateCategoryDto> createCategoryDtos)
        {
            if (createCategoryDtos is null || !createCategoryDtos.Any())
            {
                ModelState.AddModelError("CustomError", "The category collection cannot be empty.");
                return BadRequest(ModelState);
            }

            var validCategories = new List<Category>();
            var categoryNamesInPayload = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            foreach (var dto in createCategoryDtos)
            {
                if (string.IsNullOrWhiteSpace(dto.Name))
                {
                    ModelState.AddModelError("CustomError", "Category name cannot be empty.");
                    return BadRequest(ModelState);
                }

                // Validación de duplicados en la misma petición
                if (!categoryNamesInPayload.Add(dto.Name))
                {
                    ModelState.AddModelError("CustomError", $"Duplicate category name '{dto.Name}' in request body.");
                    return BadRequest(ModelState);
                }

                // Validación de existencia en Base de Datos
                if (_categoryRepository.CategoryExists(dto.Name))
                {
                    ModelState.AddModelError("CustomError", $"Category '{dto.Name}' already exists in database.");
                    return BadRequest(ModelState);
                }

                var category = _mapper.Map<Category>(dto);
                validCategories.Add(category);
            }

            if (!_categoryRepository.CreateCategories(validCategories))
            {
                ModelState.AddModelError("CustomError", "Something went wrong when saving the category records.");
                return StatusCode(500, ModelState);
            }

            var categoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(validCategories);
            return CreatedAtAction(nameof(GetCategories), categoryDtos);
        }

    }
}