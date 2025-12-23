using DataLayer.ApplicationConstant;
using DataLayer.Repositories.Interfaces;
using DataLayer.Models;

namespace DataLayer.Repositories.FileSystem
{
    public class FileUserRepository : FileRepositoryBase, IGetData<User>, IAddData<User>, ISaveAllData<User>
    {
        public FileUserRepository() : base(DataFilePaths.UsersFile) { }
        public async Task<List<User>> GetAllAsync()
        {
            var result = new List<User>();
            var lines = await ReadAllLinesAsync();
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var user = ParseUser(line);
                result.Add(user);
            }
            return result;
        }

        public async Task AddAsync(User user)
        {
            var users = await GetAllAsync();
            users.Add(user);
            await SaveAllAsync(users);
        }

        public async Task SaveAllAsync(List<User> users)
        {
            var lines = users.Select(ToLine).ToList();
            await WriteAllLinesAsync(lines);
        }

        public async Task<User?> GetByUserNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) return null;
            var users = await GetAllAsync();
            return users.SingleOrDefault(u =>
                u.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase));
        }

        internal async Task UpdateAsync(User user)
        {
            var users = await GetAllAsync();
            var index = users.FindIndex(u =>
                u.UserName.Equals(user.UserName, StringComparison.OrdinalIgnoreCase));
            if (index >= 0)
            {
                users[index] = user;
                await SaveAllAsync(users);
            }
        }

        private static User ParseUser(string line)
        {
            var parts = line.Split(',', StringSplitOptions.TrimEntries);
            var user = new User();
            if (parts.Length >= 1) user.UserName = parts[0];
            if (parts.Length >= 2) user.Password = parts[1];
            if (parts.Length >= 3 && float.TryParse(parts[2], out var wallet))
                user.Wallet = wallet;

            return user;
        }

        private static string ToLine(User user) =>
            $"{user.UserName},{user.Password},{user.Wallet}";
    }
}
