
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

        public Task<UserLoginResponseDto> Login(UserLoginDto user)
        {
            throw new NotImplementedException();
        }

        public Task<User> Register(UserRegisterDto user)
        {
            throw new NotImplementedException();
        }
    }
}