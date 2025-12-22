using Backend.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Interfaces
{
    public interface IUserService
    {
        Task<User?> ValidateUserAsync(string userName, string password);
        Task<bool> IsUserExistsAsync(string userName);
        Task CreateUserAsync(string userName, string password);
        Task UpdateUserWalletAsync(string userName, float wallet);
        Task<User?> GetUserAsync(string userName);
    }
}
