
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
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
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

        public Task<UserLoginResponseDto> Login(UserLoginDto userLoginDto)
        {
            throw new NotImplementedException();
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