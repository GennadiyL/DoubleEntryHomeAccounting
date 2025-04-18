using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Repositories.Base;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess;

public interface IRepositoryFactory
{
    T GetRepository<T>() where T : IEntityRepository;
}