using Backend.Exceptions;
using Backend.Interfaces;
using Backend.Models;
using DataLayer.Repositories.Interfaces;
using entity = DataLayer.Models;

namespace Backend.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly string _initialUserName = "Admin";
        private readonly string _initialPassword = "Admin@123";
        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }
        public async Task CreateAdminAsync(string userName, string password)
        {
            var admin = await _adminRepository.GetAllAsync();
            if(admin.Count == 0)
            {
                entity.Admin newadmin = new entity.Admin { UserName = _initialUserName, Password = _initialPassword };
                await _adminRepository.AddAsync(newadmin);
            }
        }

        public async Task<Admin?> ValidateAdminAsync(string userName, string password)
        {
            await CreateAdminAsync(userName, password);
            var admin = await _adminRepository.GetAllAsync();
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(userName))
            {
                throw new InvalidCredentialsException("Invalid Credentials");
            }

            var user = admin.SingleOrDefault(u =>
                u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase) &&
                u.Password == password);

            if (user == null) return null;
            if (admin.Count == 0) return null;
            return ToModel(admin[0]);
        }
        public Admin ToModel(entity.Admin admin) { 
            return new Admin(admin.UserName,admin.Password);
        }
    }
}
