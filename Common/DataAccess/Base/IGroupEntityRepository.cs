using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;

public interface IGroupEntityRepository<T> : IEntityRepository<T>, IGroupRepository<T>
    where T : IEntity, INamedEntity
{
}