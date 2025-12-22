namespace DataLayer.Repositories.FileSystem
{
    public abstract class FileRepositoryBase
    {
        private readonly string _filePath;

        protected FileRepositoryBase(string filePath)
        {
            EnsureFileExists();
            _filePath = filePath;
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
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }
            using var _ = File.Create(_filePath);
        }
    }
}
