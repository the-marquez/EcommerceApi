
using Asp.Versioning;
using AutoMapper;
using EcommerceApi.Constants;
using EcommerceApi.Models.Dtos;
using EcommerceApi.Repositories.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EcommerceApi.Controllers
{
    [ApiVersion("1.0")]
    [Authorize(Roles ="admin")]
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;


        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [MapToApiVersion("1.0")]
        [ResponseCache(CacheProfileName = CacheProfiles.ProfileName20s )] //20 segundos de cache
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetUsers()
        {
            var users = _userRepository.GetUsers();
            var dtos = _mapper.Map<List<UserDto>>(users);
            
            return Ok(dtos);
        }

        [HttpGet("{id:int}", Name = "GetUserById")]
        [MapToApiVersion("1.0")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetUserById(int id)
        {
            var user = _userRepository.GetUser(id);

            if( user is null)
            {
                return NotFound($"User with id {id} not found!");
            }

            var dto = _mapper.Map<UserDto>(user);

            return Ok(dto);
        }

        [HttpPost("register")]
        [MapToApiVersion("1.0")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            if( createUserDto is null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(createUserDto.UserName))
            {
                return BadRequest("Usuario Invalido!");
            }

            if( !_userRepository.IsUniqueUser(createUserDto.UserName))
            {
                return BadRequest("Este usuario ya esta en uso!");
            }

            var result = await _userRepository.Register( createUserDto );

            if( result is null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Ups!, Algo salio mal en el proceso de registro!");
            }

            return CreatedAtRoute(nameof(GetUserById), new {id=result.Id}, result);
        }

        [HttpPost("login")]
        [MapToApiVersion("1.0")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> UserLogin([FromBody] UserLoginDto userLoginDto)
        {
            if( userLoginDto is null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userRepository.Login( userLoginDto );

            if( user is null)
            {
                return Unauthorized();
            }

            return Ok(user);
        }

    }
}