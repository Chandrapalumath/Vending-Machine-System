using DataLayer.ApplicationConstant;

namespace DataLayer.Repositories.FileSystem
{
    public abstract class FileRepositoryBase
    {
        private readonly string _filePath;

        protected FileRepositoryBase(string filePath)
        {
            _filePath = filePath;
            EnsureFileExists();
        }

        protected async Task<string[]> ReadAllLinesAsync()
        {
            return await File.ReadAllLinesAsync(_filePath);
        }

        protected Task WriteAllLinesAsync(List<string> lines)
        {
            return File.WriteAllLinesAsync(_filePath, lines);
        }

        private void EnsureFileExists()
        {
            if (File.Exists(_filePath))
                return;

            var dir = Path.GetDirectoryName(_filePath);
            if (!string.IsNullOrEmpty(DataFilePaths.DataFolder) && !Directory.Exists(DataFilePaths.DataFolder))
            {
                Directory.CreateDirectory(DataFilePaths.DataFolder);
            }
            File.Create(_filePath);
        }
    }
}
