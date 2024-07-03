namespace GLSoft.DoubleEntryHomeAccounting.Common.Infrastructure.Peaa
{
    public interface IRepositoryFactory
    {
        Task<T> GetRepository<T>();
    }
}
