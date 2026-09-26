
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EcommerceApi.Data;
using EcommerceApi.Models;
using EcommerceApi.Models.Dtos;
using EcommerceApi.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace EcommerceApi.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private string? _secretKey;
        public UserRepository(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _secretKey = configuration.GetValue<string>("ApiSettings:SecretKey");
        }

        public User? GetUser(int id)
        {
            return _context.Users.FirstOrDefault((usr)=> usr.Id == id);
        }

        public ICollection<User> GetUsers()
        {
            return _context.Users.OrderBy((usr)=> usr.Name ).ToList();
        }

        public bool IsUniqueUser(string username)
        {
            return _context.Users.Any( (usr) => usr.UserName.Trim().ToLower() == username.Trim().ToLower());
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

            var user = await _context.Users.FirstOrDefaultAsync<User>( usr => usr.UserName.ToLower().Trim() == userLoginDto.UserName.ToLower().Trim());

            if( user is null)
            {
                return new UserLoginResponseDto
                {
                    Token = "",
                    User = null,
                    Message = "El nombre de usuario no fue encontrado!"
                };
            }

            if( !BCrypt.Net.BCrypt.Verify(userLoginDto.Password, user.Password))
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

            var key = Encoding.UTF8.GetBytes(_secretKey!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("id", user.Id.ToString()),
                    new Claim("username", user.UserName),
                    new Claim(ClaimTypes.Role, user.Role ?? "")
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new UserLoginResponseDto
            {
                Token = tokenHandler.WriteToken(token),
                User = new UserRegisterDto
                {
                    UserName = user.UserName,
                    Name = user.Name,
                    Password = user.Password ?? ""
                },
                Message = "Usuario autenticado correctamente!"
            };

        }

        public async Task<User> Register(CreateUserDto createUserDto)
        {
            var encriptedPassword = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password);
            var user = new User
            {
                Name = createUserDto.Name,
                UserName = createUserDto.UserName ?? "No Username",
                Role = createUserDto.Role,
                Password = encriptedPassword
            };

            _context.Users.Add( user );
            await _context.SaveChangesAsync();

            return user;
        }
    }
}