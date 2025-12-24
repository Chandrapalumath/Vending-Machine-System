using DataLayer.ApplicationConstant;
using DataLayer.Models;
using DataLayer.Repositories.Interfaces;

namespace DataLayer.Repositories.FileSystem
{
    public class FileAdminRepository : FileRepositoryBase, IAdminRepository
    {
        public FileAdminRepository() : base(DataFilePaths.AdminFile) { }
        public async Task<List<Admin>> GetAllAsync()
        {
            var result = new List<Admin>();
            var lines = await ReadAllLinesAsync();
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                var user = ParseAdmin(line);
                result.Add(user);
            }
            return result;
        }

        public async Task AddAsync(Admin admin)
        {
            var lines = new List<string>() { $"{admin.UserName},{admin.Password}"};
            await WriteAllLinesAsync(lines);
        }
        private static Admin ParseAdmin(string line)
        {
            var parts = line.Split(',', StringSplitOptions.TrimEntries);
            var admin = new Admin();
            if (parts.Length >= 1) admin.UserName = parts[0];
            if (parts.Length >= 2) admin.Password = parts[1];
            return admin;
        }
    }
}
