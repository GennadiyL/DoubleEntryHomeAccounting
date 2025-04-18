namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;

public interface IStoreManager
{
    Task RejectChanges();
    Task SaveChanges();
}