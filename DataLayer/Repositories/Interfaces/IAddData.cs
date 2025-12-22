namespace DataLayer.Repositories.Interfaces
{
    public interface IAddData<T> where T : class
    {
        Task AddAsync(T entity);
    }
}
