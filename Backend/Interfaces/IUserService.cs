using Backend.Model;

namespace Backend.Interfaces
{
    public interface IUserService
    {
        Task<User?> ValidateUserAsync(string userName, string password);
        Task<bool> IsUserExistsAsync(string userName);
        Task CreateUserAsync(string userName, string password);
        Task UpdateUserWalletAsync(string userName, float wallet);
        Task<User?> GetUserAsync(string userName);
        Task UpdatePasswordAsync(string userName, string newPassword);
    }
}
