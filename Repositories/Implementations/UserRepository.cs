
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using EcommerceApi.Data;
using EcommerceApi.Models;
using EcommerceApi.Models.Dtos;
using EcommerceApi.Repositories.Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EcommerceApi.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _context;
        private string? _secretKey;
        public UserRepository(
            UserManager<ApplicationUser> userManager, 
            RoleManager<IdentityRole> roleManager, 
            IMapper mapper, 
            ApplicationDbContext context, 
            IConfiguration configuration
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _context = context;
            _secretKey = configuration.GetValue<string>("Security:Jwt:Key");
        }

        public ApplicationUser? GetUser(string id)
        {
            return _context.ApplicationUsers.FirstOrDefault((usr)=> usr.Id == id);
        }

        public ICollection<ApplicationUser> GetUsers()
        {
            return _context.ApplicationUsers.OrderBy((usr)=> usr.UserName ).ToList();
        }

        public bool IsUniqueUser(string username)
        {
            return !_context.ApplicationUsers.Any( (usr) => usr.UserName!.Trim().ToLower() == username.Trim().ToLower());
        }

        public async Task<UserLoginResponseDto> Login(UserLoginDto userLoginDto)
        {
            if( string.IsNullOrEmpty(userLoginDto.UserName))
            {
                return new UserLoginResponseDto
                {
                    Token = "",
                    User = null,
                    Message = "El nombre de usuario es requerido!"
                };
            }

            // var user = await _context.Users.FirstOrDefaultAsync<User>( usr => usr.UserName.ToLower().Trim() == userLoginDto.UserName.ToLower().Trim());
            var user = await _context.ApplicationUsers.FirstOrDefaultAsync<ApplicationUser>(
                usr => !string.IsNullOrEmpty(usr.UserName) && 
                usr.UserName.ToLower().Trim() == userLoginDto.UserName.ToLower().Trim()
            );

            if( user is null)
            {
                return new UserLoginResponseDto
                {
                    Token = "",
                    User = null,
                    Message = "El nombre de usuario no fue encontrado!"
                };
            }

            if( string.IsNullOrWhiteSpace( userLoginDto.Password))
            {
                return new UserLoginResponseDto
                {
                    Token = "",
                    User = null,
                    Message = "La contraseña es requerida!"
                };
            }

            bool isPasswordValid = await _userManager.CheckPasswordAsync(user, userLoginDto.Password!);

            if( !isPasswordValid)
            {
                return new UserLoginResponseDto
                {
                    Token = "",
                    User = null,
                    Message = "Credenciales Incorrectas!"
                };
            }

            var tokenHandler = new JwtSecurityTokenHandler();

            if (string.IsNullOrWhiteSpace(_secretKey))
            {
                throw new InvalidOperationException("Secret Key No Configurada!");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var key = Encoding.UTF8.GetBytes(_secretKey!);
            
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", user.Id.ToString()),
                    new Claim("username", user.UserName ?? string.Empty ),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault() ?? string.Empty )
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new UserLoginResponseDto
            {
                Token = tokenHandler.WriteToken(token),
                User = _mapper.Map<UserDataDto>(user),
                Message = "Usuario autenticado correctamente!"
            };

        }

        public async Task<UserDataDto> Register(CreateUserDto createUserDto)
        {
            if( string.IsNullOrWhiteSpace(createUserDto.UserName) || 
                string.IsNullOrWhiteSpace(createUserDto.Password) )
            {
                throw new ArgumentException("Los campos de nombre de usuario y contraseña son obligatorios.");
            }

            var user = new ApplicationUser
            {
                UserName = createUserDto.UserName,
                Email = createUserDto.UserName,
                NormalizedEmail = createUserDto.UserName.ToUpper(),
                Name = createUserDto.Name ?? string.Empty,
            };

            var result = await _userManager.CreateAsync(user, createUserDto.Password);

            if( !result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Error al crear el usuario: {errors}");
            }

            var userRole = createUserDto.Role ?? "User";
            if( !await _roleManager.RoleExistsAsync(userRole))
            {
                await _roleManager.CreateAsync(new IdentityRole(userRole));
            }

            await _userManager.AddToRoleAsync(user, userRole);

            var createdUser = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.UserName == createUserDto.UserName);

            return _mapper.Map<UserDataDto>(createdUser);
        }
    }
}