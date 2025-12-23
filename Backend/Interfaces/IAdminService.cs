using Backend.Models;

namespace Backend.Interfaces
{
    public interface IAdminService
    {
        Task<Admin?> ValidateAdminAsync(string userName, string password);
        Task CreateAdminAsync(string userName, string password);
    }
}
