using GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;
using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.Infrastructure.Peaa
{
    public interface IRepositoryFactory
    {
        T GetRepository<T>() where T : IEntityRepository;
    }
}
