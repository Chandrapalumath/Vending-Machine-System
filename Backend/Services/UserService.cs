using Backend.Interfaces;
using DataLayer.Repositories.Interfaces;
using entity = DataLayer.Models;
using Backend.Model;
using Backend.Exceptions;

namespace Backend.Services
{
    public class UserService : IUserService
    {
        private readonly IGetData<entity.User> _getUsers;
        private readonly IAddData<entity.User> _addUsers;
        private readonly ISaveAllData<entity.User> _saveUsers;
        private const int MinUserNameLength = 3;
        private const int MaxUserNameLength = 30;
        private const int MinPasswordLength = 12;
        private const int MaxPasswordLength = 20;

        public UserService( IGetData<entity.User> getUsers, IAddData<entity.User> addUsers, ISaveAllData<entity.User> saveUsers)
        {
            _getUsers = getUsers;
            _addUsers = addUsers;
            _saveUsers = saveUsers;
        }

        public async Task<User?> ValidateUserAsync(string userName, string password)
        {
            if (!IsValidUserName(userName) || !IsValidPassword(password))
                return null;

            var users = await _getUsers.GetAllAsync();
            var user = users.SingleOrDefault(u =>
                u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase) &&
                u.Password == password);
            if(user == null) return null;
            return ToModel(user);
        }

        public async Task<bool> IsUserExistsAsync(string userName)
        {
            var users = await _getUsers.GetAllAsync();
            return users.Any(u => u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
        }

        public async Task CreateUserAsync(string userName, string password)
        {
            if (!IsValidUserName(userName))
                throw new InvalidCredentialsException("Invalid username format or length.");
            if (!IsValidPassword(password))
                throw new InvalidCredentialsException("Invalid password format or length.");
            if (await IsUserExistsAsync(userName))
                throw new UserNotFoundException("Username already exists.");

            var newUser = new entity.User
            {
                UserName = userName.Trim(),
                Password = password.Trim(),
                Wallet = 0f
            };

            await _addUsers.AddAsync(newUser);
        }

        public async Task UpdateUserWalletAsync(string userName, float wallet)
        {
            var users = await _getUsers.GetAllAsync();
            var user = users.SingleOrDefault(u => u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));

            if (user == null)
                throw new UserNotFoundException("User not found.");

            user.Wallet = wallet;
            await _saveUsers.SaveAllAsync(users);
        }

        public async Task<User?> GetUserAsync(string userName)
        {
            var users = await _getUsers.GetAllAsync();
            var user = users.SingleOrDefault(u => u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
            if (user == null) return null;
            return ToModel(user);
        }

        private static User ToModel(entity.User entity) =>
            new User(entity.UserName, entity.Password, entity.Wallet);

        private static bool IsValidUserName(string userName)
        {
            userName = userName.Trim();
            return !string.IsNullOrWhiteSpace(userName) &&
                   userName.Length >= MinUserNameLength && userName.Length <= MaxUserNameLength &&
                   !userName.Contains(',');
        }

        private static bool IsValidPassword(string password)
        {
            password = password.Trim();
            return !string.IsNullOrWhiteSpace(password) &&
                   password.Length >= MinPasswordLength && password.Length <= MaxPasswordLength &&
                   !password.Contains(',') && !password.Contains(' ');
        }
    }
}
