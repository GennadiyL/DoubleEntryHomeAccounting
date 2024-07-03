using GLSoft.DoubleEntryHomeAccounting.Common.Models.Interfaces;

namespace GLSoft.DoubleEntryHomeAccounting.Common.DataAccess.Base;

public interface IElementEntityRepository<T> : IEntityRepository<T>, IElementRepository<T>
    where T : IEntity, INamedEntity
{
}