using Backend.Exceptions;
using Backend.Interfaces;
using Backend.Models;
using DataLayer.Repositories.Interfaces;
using DataLayer_Models = DataLayer.Models;

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
        public async Task CreateAdminAsync()
        {
            var admin = await _adminRepository.GetAllAsync();
            if(admin.Count == 0)
            {
                DataLayer_Models.Admin newadmin = new DataLayer_Models.Admin { UserName = _initialUserName, Password = _initialPassword };
                await _adminRepository.AddAsync(newadmin);
            }
        }

        public async Task<Admin?> ValidateAdminAsync(string? userName, string? password)
        {
            if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidCredentialsException("Invalid Credentials");
            }
            var admin = await _adminRepository.GetAllAsync();
            var user = admin.SingleOrDefault(u => u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase) && u.Password == password);

            if (user == null) return null;
            if (admin.Count == 0) return null;
            return ToModel(admin[0]);
        }
        public async Task UpdatePasswordAsync(string? password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                throw new InvalidCredentialsException("Invalid Credentials");
            }
            var admin = await _adminRepository.GetAllAsync();
            var user = admin.SingleOrDefault();
            if (user == null) throw new InvalidCredentialsException("Invalid Credentials");
            user.Password = password;
            await _adminRepository.AddAsync(user);
        }
        public Admin ToModel(DataLayer_Models.Admin admin) { 
            return new Admin(admin.UserName,admin.Password);
        }
    }
}
