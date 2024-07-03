namespace GLSoft.DoubleEntryHomeAccounting.Common.Infrastructure.Peaa
{
    public interface IStoreManager
    {
        Task RejectChanges();
        Task SaveChanges();
    }
}
