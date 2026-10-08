
using EcommerceApi.Models;
using EcommerceApi.Models.Dtos;

namespace EcommerceApi.Repositories.Contracts
{
    public interface IUserRepository
    {
        ICollection<ApplicationUser> GetUsers();
        ApplicationUser? GetUser(string id);
        bool IsUniqueUser(string username);
        Task<UserLoginResponseDto> Login(UserLoginDto userLoginDto);
        Task<UserDataDto> Register(CreateUserDto createUserDto);
    }
}

