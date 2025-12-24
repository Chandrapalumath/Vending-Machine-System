using Backend.Interfaces;
using DataLayer.Repositories.Interfaces;
using DataLayer_Models = DataLayer.Models;
using Backend.Model;
using Backend.Exceptions;
using Backend.ApplicationConstants;

namespace Backend.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User?> ValidateUserAsync(string userName, string password)
        {
            if (!IsValidUserName(userName) || !IsValidPassword(password))
                return null;

            var user = await _userRepository.GetByUserNameAsync(userName);
            if(user == null) return null;
            return ToModel(user);
        }

        public async Task<bool> IsUserExistsAsync(string userName)
        {
            var user = await _userRepository.GetByUserNameAsync(userName);
            if(user == null) return false;
            return true;
        }

        public async Task CreateUserAsync(string userName, string password)
        {
            if (!IsValidUserName(userName))
                throw new InvalidCredentialsException("Invalid username format or length.");
            if (!IsValidPassword(password))
                throw new InvalidCredentialsException("Invalid password format or length.");
            if (await IsUserExistsAsync(userName))
                throw new UserNotFoundException("Username already exists.");

            var newUser = new DataLayer_Models.User
            {
                UserName = userName.Trim(),
                Password = password.Trim(),
                Wallet = 0f
            };

            await _userRepository.AddAsync(newUser);
        }

        public async Task UpdateUserWalletAsync(string userName, float wallet)
        {
            var users = await _userRepository.GetAllAsync();
            var user = users.SingleOrDefault(u => u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));

            if (user == null)
                throw new UserNotFoundException("User not found.");

            user.Wallet = wallet;
            await _userRepository.SaveAllAsync(users);
        }

        public async Task<User?> GetUserAsync(string userName)
        {
            var users = await _userRepository.GetAllAsync();
            var user = users.SingleOrDefault(u => u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
            if (user == null) return null;
            return ToModel(user);
        }

        private static User ToModel(DataLayer_Models.User entity) => new User(entity.UserName, entity.Password, entity.Wallet);

        private static bool IsValidUserName(string userName)
        {
            userName = userName.Trim();
            return !string.IsNullOrWhiteSpace(userName) &&
                   userName.Length >= UserValidationRules.MinUserNameLength && userName.Length <= UserValidationRules.MaxUserNameLength &&
                   !userName.Contains(',');
        }

        private static bool IsValidPassword(string password)
        {
            password = password.Trim();
            return !string.IsNullOrWhiteSpace(password) &&
                   password.Length >= UserValidationRules.MinPasswordLength && password.Length <= UserValidationRules.MaxPasswordLength &&
                   !password.Contains(',') && !password.Contains(' ');
        }
        public async Task UpdatePasswordAsync(string userName, string newPassword)
        {
            if (!IsValidPassword(newPassword))
                throw new ArgumentException("Invalid password format or length.");

            var users = await _userRepository.GetAllAsync();
            var userEntity = users.SingleOrDefault(u =>
                u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));

            if (userEntity == null)
                throw new InvalidOperationException("User not found.");

            userEntity.Password = newPassword.Trim();
            await _userRepository.SaveAllAsync(users);
        }

    }
}
