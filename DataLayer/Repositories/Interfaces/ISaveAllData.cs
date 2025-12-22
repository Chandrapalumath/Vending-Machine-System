namespace DataLayer.Repositories.Interfaces
{
    public interface ISaveAllData<T> where T : class
    {
        Task SaveAllAsync(List<T> entities);
    }
}
