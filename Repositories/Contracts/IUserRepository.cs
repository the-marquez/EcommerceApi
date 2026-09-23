
using EcommerceApi.Models;
using EcommerceApi.Models.Dtos;

namespace EcommerceApi.Repositories.Contracts
{
    public interface IUserRepository
    {
        ICollection<User> GetUsers();
        User? GetUser(int id);
        bool IsUniqueUser(string username);
        Task<UserLoginResponseDto> Login(UserLoginDto user);
        Task<User> Register(UserRegisterDto user);
    }
}

