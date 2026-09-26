
using EcommerceApi.Data;
using EcommerceApi.Models;
using EcommerceApi.Models.Dtos;
using EcommerceApi.Repositories.Contracts;
using Microsoft.EntityFrameworkCore;

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